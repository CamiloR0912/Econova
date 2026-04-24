using System;
using System.Windows;
using System.Windows.Input;
using Econova.Core;
using Econova.Models;

namespace Econova.ViewModels
{
    public class VentanaEditarClienteViewModel : ObservableObject
    {
        private Cliente _cliente;
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

        public ICommand GuardarCommand { get; }
        public ICommand CerrarCommand { get; }

        public event Action<bool?> CloseRequest;

        public VentanaEditarClienteViewModel(Cliente cliente)
        {
            _cliente = cliente;
            Nombres = cliente.Nombres;
            Apellidos = cliente.Apellidos;
            Cedula = cliente.Cedula;
            Telefono = cliente.Telefono;
            Email = cliente.Email;
            Direccion = cliente.Direccion;

            GuardarCommand = new RelayCommand(o => Guardar());
            CerrarCommand = new RelayCommand(o => CloseRequest?.Invoke(false));
        }

        private void Guardar()
        {
            if (string.IsNullOrWhiteSpace(Nombres) || string.IsNullOrWhiteSpace(Apellidos))
            {
                MessageBox.Show("Los nombres y apellidos son obligatorios.",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _cliente.Nombres = Nombres;
            _cliente.Apellidos = Apellidos;
            _cliente.Telefono = Telefono;
            _cliente.Email = Email;
            _cliente.Direccion = Direccion;

            // Guardar en base de datos
            var db = Econova.Infrastructure.SqliteDataService.Instance;
            bool exito = db.ActualizarCliente(_cliente, out string error);

            if (exito)
            {
                CloseRequest?.Invoke(true);
            }
            else
            {
                MessageBox.Show($"No se pudieron guardar los cambios.\n{error}",
                    "Error al guardar", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
