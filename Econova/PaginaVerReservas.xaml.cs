using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Econova
{
    // Modelo de reserva
    public class Reserva
    {
        public int    Id           { get; set; }
        public int    Numero       { get; set; }
        public string Sala         { get; set; }
        public string Cliente      { get; set; }
        public string Cedula       { get; set; }
        public DateTime FechaEntradaDt { get; set; }
        public DateTime FechaSalidaDt  { get; set; }
        public string FechaEntrada { get; set; }
        public string HoraEntrada  { get; set; }
        public string FechaSalida  { get; set; }
        public string HoraSalida   { get; set; }
    }

    public partial class PaginaVerReservas : Page
    {
        private DateTime _diaActual = DateTime.Today;
        private List<Reserva> _todasReservas = new List<Reserva>();
        private List<Reserva> _reservasFiltradas = new List<Reserva>();

        // Hora de inicio del calendario (06:00) y altura de cada franja (56px)
        private const int HoraInicio  = 6;
        private const double AlturaHora = 56.0;

        public PaginaVerReservas()
        {
            InitializeComponent();
            CargarReservasEjemplo();
            ActualizarEtiquetaDia();
            DibujarEventosCalendario();
        }

        // ── Datos de ejemplo — reemplaza con tu consulta a base de datos ──
        private void CargarReservasEjemplo()
        {
            _todasReservas = new List<Reserva>
            {
                new Reserva
                {
                    Id = 1, Numero = 1,
                    Sala = "Sala 1",
                    Cliente = "Juan Pérez",
                    Cedula = "1234567890",
                    FechaEntradaDt = DateTime.Today.AddHours(8),
                    FechaSalidaDt  = DateTime.Today.AddHours(10),
                    FechaEntrada = DateTime.Today.ToString("dd/MM/yyyy"),
                    HoraEntrada  = "08:00 AM",
                    FechaSalida  = DateTime.Today.ToString("dd/MM/yyyy"),
                    HoraSalida   = "10:00 AM"
                },
                new Reserva
                {
                    Id = 2, Numero = 2,
                    Sala = "Sala 2",
                    Cliente = "María López",
                    Cedula = "0987654321",
                    FechaEntradaDt = DateTime.Today.AddHours(11),
                    FechaSalidaDt  = DateTime.Today.AddHours(13),
                    FechaEntrada = DateTime.Today.ToString("dd/MM/yyyy"),
                    HoraEntrada  = "11:00 AM",
                    FechaSalida  = DateTime.Today.ToString("dd/MM/yyyy"),
                    HoraSalida   = "01:00 PM"
                },
                new Reserva
                {
                    Id = 3, Numero = 3,
                    Sala = "Sala 3",
                    Cliente = "Carlos Ruiz",
                    Cedula = "1122334455",
                    FechaEntradaDt = DateTime.Today.AddDays(1).AddHours(9),
                    FechaSalidaDt  = DateTime.Today.AddDays(1).AddHours(12),
                    FechaEntrada = DateTime.Today.AddDays(1).ToString("dd/MM/yyyy"),
                    HoraEntrada  = "09:00 AM",
                    FechaSalida  = DateTime.Today.AddDays(1).ToString("dd/MM/yyyy"),
                    HoraSalida   = "12:00 PM"
                },
                new Reserva
                {
                    Id = 4, Numero = 4,
                    Sala = "Salón principal",
                    Cliente = "Ana Torres",
                    Cedula = "5566778899",
                    FechaEntradaDt = DateTime.Today.AddHours(14),
                    FechaSalidaDt  = DateTime.Today.AddHours(17),
                    FechaEntrada = DateTime.Today.ToString("dd/MM/yyyy"),
                    HoraEntrada  = "02:00 PM",
                    FechaSalida  = DateTime.Today.ToString("dd/MM/yyyy"),
                    HoraSalida   = "05:00 PM"
                }
            };

            _reservasFiltradas = new List<Reserva>(_todasReservas);
            ActualizarContador();
        }

        // ── Actualiza la etiqueta del día en la vista calendario ──
        private void ActualizarEtiquetaDia()
        {
            string[] diasSemana = { "Domingo", "Lunes", "Martes", "Miércoles",
                                    "Jueves", "Viernes", "Sábado" };
            string[] meses = { "enero", "febrero", "marzo", "abril", "mayo", "junio",
                                "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre" };

            TxtFechaDia.Text   = $"{_diaActual.Day} de {meses[_diaActual.Month - 1]} de {_diaActual.Year}";
            TxtDiaSemana.Text  = diasSemana[(int)_diaActual.DayOfWeek];
            TxtSubtitulo.Text  = _diaActual.Date == DateTime.Today ? "Hoy" :
                                 _diaActual.Date == DateTime.Today.AddDays(1) ? "Mañana" :
                                 _diaActual.ToString("dd/MM/yyyy");
        }

        // ── Dibuja los eventos del día actual en el Canvas ──
        private void DibujarEventosCalendario()
        {
            CanvasEventos.Children.Clear();

            var reservasDelDia = _todasReservas
                .Where(r => r.FechaEntradaDt.Date == _diaActual.Date)
                .ToList();

            // Paleta de colores para los eventos
            string[] fondos   = { "#DBEAFE", "#DCFCE7", "#FEF3C7", "#FCE7F3", "#EDE9FE" };
            string[] bordes   = { "#93C5FD", "#86EFAC", "#FCD34D", "#F9A8D4", "#C4B5FD" };
            string[] textos   = { "#1D4ED8", "#15803D", "#92400E", "#9D174D", "#6D28D9" };

            for (int i = 0; i < reservasDelDia.Count; i++)
            {
                var r = reservasDelDia[i];
                int color = i % fondos.Length;

                // Calcular posición y altura en el canvas
                double horaInicioPx = (r.FechaEntradaDt.Hour + r.FechaEntradaDt.Minute / 60.0 - HoraInicio) * AlturaHora;
                double duracionHoras = (r.FechaSalidaDt - r.FechaEntradaDt).TotalHours;
                double alturaEvento  = Math.Max(duracionHoras * AlturaHora, 40);

                // Contenedor del evento
                var borde = new Border
                {
                    Background      = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fondos[color])),
                    BorderBrush     = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bordes[color])),
                    BorderThickness = new Thickness(3, 1, 1, 1),
                    CornerRadius    = new CornerRadius(6),
                    Padding         = new Thickness(8, 4, 8, 4),
                    Margin          = new Thickness(4, 0, 8, 0),
                    Cursor          = System.Windows.Input.Cursors.Hand,
                    Tag             = r.Id,
                    Width           = double.NaN,
                    Height          = alturaEvento
                };

                var stack = new StackPanel();

                stack.Children.Add(new TextBlock
                {
                    Text       = r.Sala,
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize   = 12,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(textos[color])),
                    TextTrimming = TextTrimming.CharacterEllipsis
                });

                stack.Children.Add(new TextBlock
                {
                    Text       = $"{r.HoraEntrada} – {r.HoraSalida}",
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize   = 11,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(textos[color])),
                    Opacity    = 0.8
                });

                stack.Children.Add(new TextBlock
                {
                    Text       = r.Cliente,
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize   = 11,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(textos[color])),
                    Opacity    = 0.7,
                    TextTrimming = TextTrimming.CharacterEllipsis
                });

                borde.Child = stack;
                borde.MouseLeftButtonUp += (s, e) => MostrarDetalle(r);

                Canvas.SetTop(borde, Math.Max(horaInicioPx, 0));
                Canvas.SetLeft(borde, 0);

                // Hacer que el borde ocupe el ancho del canvas
                borde.SetBinding(Border.WidthProperty,
                    new System.Windows.Data.Binding("ActualWidth")
                    {
                        Source = CanvasEventos
                    });

                CanvasEventos.Children.Add(borde);
            }

            // Si no hay reservas, mostrar mensaje
            if (!reservasDelDia.Any())
            {
                var msg = new TextBlock
                {
                    Text       = "No hay reservas para este día.",
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize   = 14,
                    Foreground = new SolidColorBrush(Color.FromRgb(156, 163, 175)),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Canvas.SetTop(msg, 200);
                Canvas.SetLeft(msg, 0);
                msg.SetBinding(TextBlock.WidthProperty,
                    new System.Windows.Data.Binding("ActualWidth")
                    { Source = CanvasEventos });
                msg.TextAlignment = TextAlignment.Center;
                CanvasEventos.Children.Add(msg);
            }
        }

        // ── Mostrar detalle de una reserva ──
        private void MostrarDetalle(Reserva r)
        {
            MessageBox.Show(
                $"Sala:       {r.Sala}\n" +
                $"Cliente:    {r.Cliente}\n" +
                $"Cédula:     {r.Cedula}\n\n" +
                $"Entrada:    {r.FechaEntrada}  {r.HoraEntrada}\n" +
                $"Salida:     {r.FechaSalida}  {r.HoraSalida}",
                "Detalle de reserva",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // ── Actualizar contador de la tabla ──
        private void ActualizarContador()
        {
            int total = _reservasFiltradas.Count;
            TxtContador.Text = total == 1 ? "1 reserva" : $"{total} reservas";
            ListaReservas.ItemsSource = null;
            ListaReservas.ItemsSource = _reservasFiltradas;
        }

        // ────────────── EVENTOS DE UI ──────────────

        // Cambiar a vista calendario
        private void BtnVistaCalendario_Click(object sender, RoutedEventArgs e)
        {
            PanelCalendario.Visibility     = Visibility.Visible;
            PanelTodasReservas.Visibility  = Visibility.Collapsed;
            BtnVistaCalendario.Style = (Style)FindResource("ViewButtonActiveStyle");
            BtnVistaTodas.Style      = (Style)FindResource("ViewButtonStyle");
            TxtSubtitulo.Text = "Vista del día";
        }

        // Cambiar a vista todas las reservas
        private void BtnVistaTodas_Click(object sender, RoutedEventArgs e)
        {
            PanelCalendario.Visibility    = Visibility.Collapsed;
            PanelTodasReservas.Visibility = Visibility.Visible;
            BtnVistaTodas.Style      = (Style)FindResource("ViewButtonActiveStyle");
            BtnVistaCalendario.Style = (Style)FindResource("ViewButtonStyle");
            TxtSubtitulo.Text = "Todas las reservas";
            ActualizarContador();
        }

        // Día anterior
        private void BtnDiaAnterior_Click(object sender, RoutedEventArgs e)
        {
            _diaActual = _diaActual.AddDays(-1);
            ActualizarEtiquetaDia();
            DibujarEventosCalendario();
        }

        // Día siguiente
        private void BtnDiaSiguiente_Click(object sender, RoutedEventArgs e)
        {
            _diaActual = _diaActual.AddDays(1);
            ActualizarEtiquetaDia();
            DibujarEventosCalendario();
        }

        // Buscar en la tabla
        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string q = TxtBuscar.Text.Trim().ToLower();

            _reservasFiltradas = string.IsNullOrEmpty(q)
                ? new List<Reserva>(_todasReservas)
                : _todasReservas.Where(r =>
                    r.Sala.ToLower().Contains(q)    ||
                    r.Cliente.ToLower().Contains(q) ||
                    r.Cedula.Contains(q)).ToList();

            ActualizarContador();
        }

        // Ver detalle desde tabla
        private void BtnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            var r  = _todasReservas.FirstOrDefault(x => x.Id == id);
            if (r != null) MostrarDetalle(r);
        }

        // Editar reserva
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            var r  = _todasReservas.FirstOrDefault(x => x.Id == id);
            if (r == null) return;

            MessageBox.Show(
                $"Aquí abrirás el formulario de edición para:\n\n" +
                $"  Sala: {r.Sala}\n  Cliente: {r.Cliente}",
                "Editar reserva",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            // Cuando tengas la página de edición:
            // this.NavigationService.Navigate(new PaginaEditarReserva(id));
        }

        // Cancelar reserva
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            var r  = _todasReservas.FirstOrDefault(x => x.Id == id);
            if (r == null) return;

            var resultado = MessageBox.Show(
                $"¿Estás seguro de que deseas cancelar esta reserva?\n\n" +
                $"  Sala:     {r.Sala}\n" +
                $"  Cliente:  {r.Cliente}\n" +
                $"  Entrada:  {r.FechaEntrada}  {r.HoraEntrada}",
                "Cancelar reserva",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                // ── Aquí eliminarás la reserva de tu base de datos ──
                _todasReservas.Remove(r);
                _reservasFiltradas.Remove(r);

                // Renumerar
                for (int i = 0; i < _todasReservas.Count; i++)
                    _todasReservas[i].Numero = i + 1;

                ActualizarContador();
                DibujarEventosCalendario();

                MessageBox.Show("Reserva cancelada exitosamente.",
                    "Cancelación confirmada",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }
}
