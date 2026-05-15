using AForge.Video;
using AForge.Video.DirectShow;
using Econova.Core;
using Econova.Infrastructure;
using Econova.Models;
using Econova.Services;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SysImaging = System.Drawing.Imaging;

namespace Econova.ViewModels
{
    public class PaginaCrearClienteViewModel : ObservableObject
    {
        private readonly SqliteDataService _db = SqliteDataService.Instance;
        private readonly OcrService _ocr = new OcrService();

        // ── Campos del formulario ────────────────────────────────────────
        private string _nombres = string.Empty;
        private string _apellidos = string.Empty;
        private string _cedula = string.Empty;
        private string _telefono = string.Empty;
        private string _email = string.Empty;
        private string _direccion = string.Empty;

        public string Nombres { get => _nombres; set => SetProperty(ref _nombres, value); }
        public string Apellidos { get => _apellidos; set => SetProperty(ref _apellidos, value); }
        public string Cedula { get => _cedula; set => SetProperty(ref _cedula, value); }
        public string Telefono { get => _telefono; set => SetProperty(ref _telefono, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string Direccion { get => _direccion; set => SetProperty(ref _direccion, value); }

        // ── Estado del panel OCR ─────────────────────────────────────────
        private bool _ocrVisible = false;
        private bool _ocrProcesando = false;
        private string _ocrEstado = "Captura o selecciona una foto de la cédula.";
        private ImageSource _ocrPreview;
        private Bitmap _imagenOcr;

        // Cámara (AForge)
        private VideoCaptureDevice _camara;
        private Bitmap _frameActual;
        private readonly object _lockFrame = new object();

        public bool OcrVisible
        {
            get => _ocrVisible;
            set
            {
                SetProperty(ref _ocrVisible, value);
                // Al cerrar el panel, detener la cámara si estaba activa
                if (!value) DetenerCamara();
            }
        }

        public bool OcrProcesando
        {
            get => _ocrProcesando;
            set => SetProperty(ref _ocrProcesando, value);
        }

        public string OcrEstado
        {
            get => _ocrEstado;
            set => SetProperty(ref _ocrEstado, value);
        }

        public ImageSource OcrPreview
        {
            get => _ocrPreview;
            set => SetProperty(ref _ocrPreview, value);
        }

        // ── Comandos ─────────────────────────────────────────────────────
        public ICommand LimpiarCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand ToggleOcrCommand { get; }
        public ICommand SeleccionarFotoCommand { get; }
        public ICommand CapturarCamaraCommand { get; }
        public ICommand ProcesarOcrCommand { get; }

        public PaginaCrearClienteViewModel()
        {
            LimpiarCommand = new RelayCommand(o => Limpiar());
            GuardarCommand = new RelayCommand(o => Guardar());
            ToggleOcrCommand = new RelayCommand(o => OcrVisible = !OcrVisible);
            SeleccionarFotoCommand = new RelayCommand(o => SeleccionarFoto());
            CapturarCamaraCommand = new RelayCommand(o => CapturarCamara());
            ProcesarOcrCommand = new RelayCommand(
                o => ProcesarOcr(),
                o => _imagenOcr != null && !OcrProcesando);
        }

        // ── OCR: seleccionar foto ────────────────────────────────────────

        private void SeleccionarFoto()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp;*.tiff",
                Title = "Selecciona la foto de la cédula"
            };
            if (dlg.ShowDialog() != true) return;

            DetenerCamara();
            _imagenOcr = new Bitmap(dlg.FileName);
            OcrPreview = BitmapToImageSource(_imagenOcr);
            OcrEstado = "Foto cargada. Presiona 'Leer cédula'.";
        }

        // ── OCR: capturar desde cámara (AForge) ─────────────────────────

        private void CapturarCamara()
        {
            var camaras = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (camaras.Count == 0)
            {
                MessageBox.Show("No se encontró una cámara conectada.",
                    "Cámara", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            OcrEstado = "Iniciando cámara... espera un momento y vuelve a presionar 'Usar cámara' para capturar.";

            if (_camara == null || !_camara.IsRunning)
            {
                // Primera pulsación: iniciar cámara y mostrar feed
                _camara = new VideoCaptureDevice(camaras[0].MonikerString);
                _camara.NewFrame += Camara_NewFrame;
                _camara.Start();
                OcrEstado = "Cámara activa. Encuadra la cédula y presiona 'Usar cámara' de nuevo para capturar.";
            }
            else
            {
                // Segunda pulsación: capturar el frame actual
                lock (_lockFrame)
                {
                    if (_frameActual != null)
                    {
                        _imagenOcr = (Bitmap)_frameActual.Clone();
                        OcrPreview = BitmapToImageSource(_imagenOcr);
                    }
                }
                DetenerCamara();
                OcrEstado = "Imagen capturada. Presiona 'Leer cédula'.";
            }
        }

        private void Camara_NewFrame(object sender, NewFrameEventArgs e)
        {
            // Clonar y convertir a 32bpp AQUÍ, en el hilo de AForge,
            // antes de cruzar al hilo de UI. Si se hace después, el bitmap
            // original ya fue liberado por AForge y lanza ArgumentException.
            Bitmap convertido = null;
            try
            {
                var frame = e.Frame;
                convertido = new Bitmap(frame.Width, frame.Height,
                                        SysImaging.PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(convertido))
                    g.DrawImage(frame, 0, 0, frame.Width, frame.Height);

                // Guardar el último frame para capturar
                lock (_lockFrame)
                {
                    _frameActual?.Dispose();
                    _frameActual = (Bitmap)convertido.Clone();
                }

                // Convertir a ImageSource en el hilo de AForge (ya es 32bpp, no falla)
                var imageSource = BitmapToImageSource(convertido);

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    OcrPreview = imageSource;
                }));
            }
            catch { /* ignorar frames con error */ }
            finally
            {
                convertido?.Dispose();
            }
        }

        private void DetenerCamara()
        {
            if (_camara != null && _camara.IsRunning)
            {
                _camara.SignalToStop();
                _camara.NewFrame -= Camara_NewFrame;
                _camara = null;
            }
        }

        // ── OCR: procesar imagen ─────────────────────────────────────────

        private async void ProcesarOcr()
        {
            if (_imagenOcr == null) return;

            OcrProcesando = true;
            OcrEstado = "Leyendo cédula...";

            try
            {
                var imagen = _imagenOcr;
                var datos = await Task.Run(() => _ocr.ProcesarImagen(imagen));

                if (!string.IsNullOrWhiteSpace(datos.Cedula)) Cedula = datos.Cedula;
                if (!string.IsNullOrWhiteSpace(datos.Nombres)) Nombres = datos.Nombres;
                if (!string.IsNullOrWhiteSpace(datos.Apellidos)) Apellidos = datos.Apellidos;

                OcrEstado = "✔ Datos cargados. Revisa y completa los campos restantes.";
                OcrVisible = false;
            }
            catch (Exception ex)
            {
                OcrEstado = "Error al leer la cédula: " + ex.Message;
            }
            finally
            {
                OcrProcesando = false;
            }
        }

        // ── Formulario ───────────────────────────────────────────────────

        private void Limpiar()
        {
            Nombres = string.Empty;
            Apellidos = string.Empty;
            Cedula = string.Empty;
            Telefono = string.Empty;
            Email = string.Empty;
            Direccion = string.Empty;

            _imagenOcr = null;
            OcrPreview = null;
            OcrEstado = "Captura o selecciona una foto de la cédula.";
            OcrVisible = false;
        }

        private void Guardar()
        {
            if (string.IsNullOrWhiteSpace(Nombres))
            {
                MessageBox.Show("Por favor ingresa los nombres del cliente.",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(Apellidos))
            {
                MessageBox.Show("Por favor ingresa los apellidos del cliente.",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(Cedula) || Cedula.Length < 6)
            {
                MessageBox.Show("Por favor ingresa una cédula válida (mínimo 6 dígitos).",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(Telefono) || Telefono.Length < 7)
            {
                MessageBox.Show("Por favor ingresa un teléfono válido (mínimo 7 dígitos).",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(Email) ||
                !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Por favor ingresa un correo electrónico válido.",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(Direccion))
            {
                MessageBox.Show("Por favor ingresa la dirección del cliente.",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var ventana = new Econova.Views.Windows.ConfirmacionClienteWindow(
                Nombres + " " + Apellidos, Cedula, Telefono, Email, Direccion)
            {
                Owner = Application.Current.MainWindow
            };
            ventana.ShowDialog();

            if (ventana.Confirmado)
            {
                var cliente = new Cliente
                {
                    Nombres = Nombres.Trim(),
                    Apellidos = Apellidos.Trim(),
                    Cedula = Cedula.Trim(),
                    Telefono = Telefono.Trim(),
                    Email = Email.Trim(),
                    Direccion = Direccion.Trim()
                };

                string error;
                if (_db.AgregarCliente(cliente, out error))
                {
                    Limpiar();
                    MessageBox.Show("Cliente guardado correctamente.",
                        "Registro exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("No se pudo guardar el cliente.\n" + error,
                        "Error de guardado", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // ── Helper ───────────────────────────────────────────────────────

        private static BitmapImage BitmapToImageSource(Bitmap bmp)
        {
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, SysImaging.ImageFormat.Png);
                ms.Position = 0;
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = ms;
                bi.EndInit();
                bi.Freeze();
                return bi;
            }
        }
    }
}