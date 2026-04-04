using System.Windows.Input;
using Econova.Core;
using Econova.Services;

namespace Econova.ViewModels
{
    public class PaginaCrearReservaViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;

        private string _cedula;
        private string _nombreCliente;
        private string _infoCliente;
        private bool _panelClienteVisible = false;
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

        // 👇 bool en lugar de Visibility
        public bool PanelClienteVisible
        {
            get => _panelClienteVisible;
            set => SetProperty(ref _panelClienteVisible, value);
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

        public PaginaCrearReservaViewModel(IDialogService dialogService) // 👈
        {
            _dialogService = dialogService;
            BuscarClienteCommand = new RelayCommand(o => BuscarCliente());
            ToggleAmPmEntradaCommand = new RelayCommand(o => AmPmEntrada = AmPmEntrada == "AM" ? "PM" : "AM");
            ToggleAmPmSalidaCommand = new RelayCommand(o => AmPmSalida = AmPmSalida == "AM" ? "PM" : "AM");
            ConfirmarReservaCommand = new RelayCommand(o => ConfirmarReserva());
        }

        private void BuscarCliente()
        {
            if (string.IsNullOrEmpty(Cedula))
            {
                _dialogService.Informar("Por favor ingresa un número de cédula.", "Campo vacío");
                return;
            }

            // Simulación de búsqueda — reemplazar con consulta real a BD
            if (Cedula == "1234567890")
            {
                NombreCliente = "Juan Pérez";
                InfoCliente = $"{Cedula} • 310 000 0000";
                PanelClienteVisible = true;
            }
            else
            {
                PanelClienteVisible = false;
                _dialogService.Informar(
                    $"No se encontró ningún cliente con la cédula {Cedula}.",
                    "Cliente no encontrado");
            }
        }

        private void ConfirmarReserva()
        {
            _dialogService.Informar("¡Reserva confirmada exitosamente!", "Éxito");
        }
    }
}