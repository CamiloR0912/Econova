using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Econova.Core;
using Econova.Infrastructure;
using Econova.Models;
using System.Windows.Navigation;

namespace Econova.ViewModels
{
    public class PaginaCrearClienteViewModel : ObservableObject
    {
        private readonly SqliteDataService _db = SqliteDataService.Instance;
        private string _nombres;
        private string _apellidos;
        private string _cedula;
        private string _telefono;
        private string _email;
        private string _direccion;

        public string Nombres
        {
            get => _nombres;
            set => SetProperty(ref _nombres, value);
        }

        public string Apellidos
        {
            get => _apellidos;
            set => SetProperty(ref _apellidos, value);
        }

        public string Cedula
        {
            get => _cedula;
            set => SetProperty(ref _cedula, value);
        }

        public string Telefono
        {
            get => _telefono;
            set => SetProperty(ref _telefono, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Direccion
        {
            get => _direccion;
            set => SetProperty(ref _direccion, value);
        }

        public ICommand LimpiarCommand { get; }
        public ICommand GuardarCommand { get; }

        public PaginaCrearClienteViewModel()
        {
            LimpiarCommand = new RelayCommand(o => Limpiar());
            GuardarCommand = new RelayCommand(o => Guardar());
        }

        private void Limpiar()
        {
            Nombres = string.Empty;
            Apellidos = string.Empty;
            Cedula = string.Empty;
            Telefono = string.Empty;
            Email = string.Empty;
            Direccion = string.Empty;
        }

        private void Guardar()
        {
            // ── Validaciones ──
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

            // ── Ventana de confirmación personalizada ──
            var ventana = new Econova.Views.Windows.ConfirmacionClienteWindow(
                $"{Nombres} {Apellidos}", Cedula, Telefono, Email, Direccion)
            {
                Owner = System.Windows.Application.Current.MainWindow
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

                if (_db.AgregarCliente(cliente, out string error))
                {
                    Limpiar();
                    MessageBox.Show("Cliente guardado correctamente.",
                        "Registro exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"No se pudo guardar el cliente.\n{error}",
                        "Error de guardado", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
