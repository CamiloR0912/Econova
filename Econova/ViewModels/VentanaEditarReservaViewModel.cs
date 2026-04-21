using System;
using System.Windows.Input;
using Econova.Core;
using Econova.Models;

namespace Econova.ViewModels
{
    public class VentanaEditarReservaViewModel : ObservableObject
    {
        private Reserva _reserva;
        private string _sala;
        private string _cliente;
        private string _cedula;
        private string _fechaEntrada;
        private string _horaEntrada;
        private string _fechaSalida;
        private string _horaSalida;

        public string Sala
        {
            get => _sala;
            set => SetProperty(ref _sala, value);
        }

        public string Cliente
        {
            get => _cliente;
            set => SetProperty(ref _cliente, value);
        }

        public string Cedula
        {
            get => _cedula;
            set => SetProperty(ref _cedula, value);
        }

        public string FechaEntrada
        {
            get => _fechaEntrada;
            set => SetProperty(ref _fechaEntrada, value);
        }

        public string HoraEntrada
        {
            get => _horaEntrada;
            set => SetProperty(ref _horaEntrada, value);
        }

        public string FechaSalida
        {
            get => _fechaSalida;
            set => SetProperty(ref _fechaSalida, value);
        }

        public string HoraSalida
        {
            get => _horaSalida;
            set => SetProperty(ref _horaSalida, value);
        }

        public ICommand GuardarCommand { get; }
        public ICommand CerrarCommand { get; }

        public event Action<bool?> CloseRequest;

        public VentanaEditarReservaViewModel(Reserva reserva)
        {
            _reserva = reserva;
            Sala = reserva.Sala;
            Cliente = reserva.Cliente;
            Cedula = reserva.Cedula;
            FechaEntrada = reserva.FechaEntrada;
            HoraEntrada = reserva.HoraEntrada;
            FechaSalida = reserva.FechaSalida;
            HoraSalida = reserva.HoraSalida;

            GuardarCommand = new RelayCommand(o => Guardar());
            CerrarCommand = new RelayCommand(o => CloseRequest?.Invoke(false));
        }

        private void Guardar()
        {
            if (string.IsNullOrWhiteSpace(Sala) || string.IsNullOrWhiteSpace(Cliente))
            {
                return;
            }

            _reserva.Sala = Sala;
            _reserva.Cliente = Cliente;
            _reserva.FechaEntrada = FechaEntrada;
            _reserva.HoraEntrada = HoraEntrada;
            _reserva.FechaSalida = FechaSalida;
            _reserva.HoraSalida = HoraSalida;

            CloseRequest?.Invoke(true);
        }
    }
}
