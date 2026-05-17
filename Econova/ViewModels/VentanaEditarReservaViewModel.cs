using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Econova.Core;
using Econova.Infrastructure;
using Econova.Models;

namespace Econova.ViewModels
{
    public class VentanaEditarReservaViewModel : ObservableObject
    {
        private readonly SqliteDataService _db = SqliteDataService.Instance;
        private Reserva _reserva;
        private int _salaIdOriginal;
        private int _clienteId;

        private ObservableCollection<Sala> _salasDisponibles;
        private Sala _salaSeleccionada;
        private string _cliente;
        private string _cedula;
        private DateTime _fechaEntrada;
        private string _horaEntradaTexto;
        private string _amPmEntrada;
        private DateTime _fechaSalida;
        private string _horaSalidaTexto;
        private string _amPmSalida;

        public ObservableCollection<Sala> SalasDisponibles
        {
            get => _salasDisponibles;
            set => SetProperty(ref _salasDisponibles, value);
        }

        public Sala SalaSeleccionada
        {
            get => _salaSeleccionada;
            set => SetProperty(ref _salaSeleccionada, value);
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

        public DateTime FechaEntrada
        {
            get => _fechaEntrada;
            set => SetProperty(ref _fechaEntrada, value);
        }

        public string HoraEntradaTexto
        {
            get => _horaEntradaTexto;
            set => SetProperty(ref _horaEntradaTexto, value);
        }

        public string AmPmEntrada
        {
            get => _amPmEntrada;
            set => SetProperty(ref _amPmEntrada, value);
        }

        public DateTime FechaSalida
        {
            get => _fechaSalida;
            set => SetProperty(ref _fechaSalida, value);
        }

        public string HoraSalidaTexto
        {
            get => _horaSalidaTexto;
            set => SetProperty(ref _horaSalidaTexto, value);
        }

        public string AmPmSalida
        {
            get => _amPmSalida;
            set => SetProperty(ref _amPmSalida, value);
        }

        public ICommand GuardarCommand { get; }
        public ICommand CerrarCommand { get; }
        public ICommand ToggleAmPmEntradaCommand { get; }
        public ICommand ToggleAmPmSalidaCommand { get; }

        public event Action<bool?> CloseRequest;

        public VentanaEditarReservaViewModel(Reserva reserva)
        {
            _reserva = reserva;

            // Cargar salas
            var salas = _db.ObtenerSalas();
            SalasDisponibles = new ObservableCollection<Sala>(salas);

            // Obtener IDs internos de la reserva
            var ids = _db.ObtenerIdsReserva(reserva.Id);
            _salaIdOriginal = ids.SalaId;
            _clienteId = ids.ClienteId;

            // Seleccionar la sala actual
            foreach (var sala in SalasDisponibles)
            {
                if (sala.Id == ids.SalaId)
                {
                    SalaSeleccionada = sala;
                    break;
                }
            }

            Cliente = reserva.Cliente;
            Cedula = reserva.Cedula;

            // Parsear fechas y horas
            FechaEntrada = reserva.FechaEntradaDt.Date;
            FechaSalida = reserva.FechaSalidaDt.Date;

            int horaEnt = reserva.FechaEntradaDt.Hour;
            AmPmEntrada = horaEnt >= 12 ? "PM" : "AM";
            int hora12Ent = horaEnt % 12;
            if (hora12Ent == 0) hora12Ent = 12;
            HoraEntradaTexto = $"{hora12Ent:D2}:{reserva.FechaEntradaDt.Minute:D2}";

            int horaSal = reserva.FechaSalidaDt.Hour;
            AmPmSalida = horaSal >= 12 ? "PM" : "AM";
            int hora12Sal = horaSal % 12;
            if (hora12Sal == 0) hora12Sal = 12;
            HoraSalidaTexto = $"{hora12Sal:D2}:{reserva.FechaSalidaDt.Minute:D2}";

            GuardarCommand = new RelayCommand(o => Guardar());
            CerrarCommand = new RelayCommand(o => CloseRequest?.Invoke(false));
            ToggleAmPmEntradaCommand = new RelayCommand(o => AmPmEntrada = AmPmEntrada == "AM" ? "PM" : "AM");
            ToggleAmPmSalidaCommand = new RelayCommand(o => AmPmSalida = AmPmSalida == "AM" ? "PM" : "AM");
        }

        private void Guardar()
        {
            if (SalaSeleccionada == null)
            {
                MessageBox.Show("Selecciona una sala.", "Campo requerido",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Parsear hora de entrada
            if (!DateTime.TryParseExact(HoraEntradaTexto?.Trim() + " " + AmPmEntrada,
                "hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime horaEntParsed))
            {
                MessageBox.Show("La hora de entrada no es válida (formato: HH:MM).",
                    "Formato inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Parsear hora de salida
            if (!DateTime.TryParseExact(HoraSalidaTexto?.Trim() + " " + AmPmSalida,
                "hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime horaSalParsed))
            {
                MessageBox.Show("La hora de salida no es válida (formato: HH:MM).",
                    "Formato inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime dtEntrada = FechaEntrada.Date.Add(horaEntParsed.TimeOfDay);
            DateTime dtSalida = FechaSalida.Date.Add(horaSalParsed.TimeOfDay);

            if (dtSalida <= dtEntrada)
            {
                MessageBox.Show("La fecha y hora de salida deben ser posteriores a la entrada.",
                    "Fechas inválidas", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // RF-04: Revalidar traslape excluyendo la reserva actual
            if (_db.ExisteTraslapeReserva(SalaSeleccionada.Id, dtEntrada, dtSalida, _reserva.Id))
            {
                MessageBox.Show("La sala ya tiene otra reserva en ese horario.\nSelecciona otro horario o sala.",
                    "Conflicto de horario", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Persistir en BD
            if (_db.ActualizarReserva(_reserva.Id, SalaSeleccionada.Id, _clienteId, dtEntrada, dtSalida, out string error))
            {
                // Actualizar el objeto local para reflejar cambios en la UI
                _reserva.Sala = SalaSeleccionada.Nombre;
                _reserva.FechaEntradaDt = dtEntrada;
                _reserva.FechaSalidaDt = dtSalida;
                _reserva.FechaEntrada = dtEntrada.ToString("dd/MM/yyyy");
                _reserva.HoraEntrada = dtEntrada.ToString("hh:mm tt");
                _reserva.FechaSalida = dtSalida.ToString("dd/MM/yyyy");
                _reserva.HoraSalida = dtSalida.ToString("hh:mm tt");

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
