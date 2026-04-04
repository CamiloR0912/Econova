using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Econova.Core;

namespace Econova.ViewModels
{
    public class PaginaCrearReservaViewModel : ObservableObject
    {
        private string _cedula;
        private string _nombreCliente;
        private string _infoCliente;
        private Visibility _panelClienteVisibility = Visibility.Collapsed;
        private string _amPmEntrada = "AM";
        private string _amPmSalida = "AM";

        public string Cedula
        {
            get => _cedula;
            set => SetProperty(ref _cedula, value);
        }

        public string NombreCliente
        {
            get => _nombreCliente;
            set => SetProperty(ref _nombreCliente, value);
        }

        public string InfoCliente
        {
            get => _infoCliente;
            set => SetProperty(ref _infoCliente, value);
        }

        public Visibility PanelClienteVisibility
        {
            get => _panelClienteVisibility;
            set => SetProperty(ref _panelClienteVisibility, value);
        }

        public string AmPmEntrada
        {
            get => _amPmEntrada;
            set => SetProperty(ref _amPmEntrada, value);
        }

        public string AmPmSalida
        {
            get => _amPmSalida;
            set => SetProperty(ref _amPmSalida, value);
        }

        public ICommand BuscarClienteCommand { get; }
        public ICommand ToggleAmPmEntradaCommand { get; }
        public ICommand ToggleAmPmSalidaCommand { get; }
        public ICommand ConfirmarReservaCommand { get; }

        public PaginaCrearReservaViewModel()
        {
            BuscarClienteCommand = new RelayCommand(o => BuscarCliente());
            ToggleAmPmEntradaCommand = new RelayCommand(o => AmPmEntrada = AmPmEntrada == "AM" ? "PM" : "AM");
            ToggleAmPmSalidaCommand = new RelayCommand(o => AmPmSalida = AmPmSalida == "AM" ? "PM" : "AM");
            ConfirmarReservaCommand = new RelayCommand(o => ConfirmarReserva());
        }

        private void BuscarCliente()
        {
            if (string.IsNullOrEmpty(Cedula))
            {
                MessageBox.Show("Por favor ingresa un número de cédula.",
                    "Campo vacío", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Cedula == "1234567890")
            {
                NombreCliente = "Juan Pérez";
                InfoCliente = $"{Cedula} • 310 000 0000";
                PanelClienteVisibility = Visibility.Visible;
            }
            else
            {
                PanelClienteVisibility = Visibility.Collapsed;
                MessageBox.Show($"No se encontró ningún cliente con la cédula {Cedula}.",
                    "Cliente no encontrado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ConfirmarReserva()
        {
            // Logic for confirmation can be added here
            MessageBox.Show("¡Reserva confirmada exitosamente!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
