using Econova.Core;
using Econova.Infrastructure;
using Econova.Models;
using Econova.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Econova.ViewModels
{
    public class PaginaCrearReservaViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        private readonly SqliteDataService _db = SqliteDataService.Instance;

        private string _cedula;
        private string _nombreCliente;
        private string _infoCliente;
        private bool _panelClienteVisible = false;
        private string _amPmEntrada = "AM";
        private string _amPmSalida = "AM";
        private ObservableCollection<string> _cedulasSugeridas = new ObservableCollection<string>();
        private ObservableCollection<Sala> _salasDisponibles = new ObservableCollection<Sala>();
        private bool _mostrarSugerenciasCedula;
        private string _cedulaSugeridaSeleccionada;
        private Sala _salaSeleccionada;
        public Action NavegaAlInicio { get; set; }

        public string Cedula
        {
            get => _cedula;
            set
            {
                if (SetProperty(ref _cedula, value))
                {
                    ActualizarSugerenciasCedula();
                }
            }
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

        public ObservableCollection<string> CedulasSugeridas
        {
            get => _cedulasSugeridas;
            set => SetProperty(ref _cedulasSugeridas, value);
        }

        public bool MostrarSugerenciasCedula
        {
            get => _mostrarSugerenciasCedula;
            set => SetProperty(ref _mostrarSugerenciasCedula, value);
        }

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

        public string CedulaSugeridaSeleccionada
        {
            get => _cedulaSugeridaSeleccionada;
            set
            {
                if (SetProperty(ref _cedulaSugeridaSeleccionada, value) &&
                    !string.IsNullOrWhiteSpace(value))
                {
                    Cedula = value;
                    OcultarSugerenciasCedula();
                    BuscarCliente(false);
                }
            }
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
            ConfirmarReservaCommand = new RelayCommand(o => { });
            CargarDatos();
        }

        private void CargarDatos()
        {
            SalasDisponibles = new ObservableCollection<Sala>(_db.ObtenerSalas());
        }

        private void BuscarCliente(bool mostrarDialogoSiNoExiste = true)
        {
            if (string.IsNullOrEmpty(Cedula))
            {
                _dialogService.Informar("Por favor ingresa un número de cédula.", "Campo vacío");
                return;
            }

            var clienteEncontrado = _db.BuscarClientePorCedula(Cedula);
            if (clienteEncontrado != null)
            {
                NombreCliente = clienteEncontrado.NombreCompleto;
                InfoCliente = $"{clienteEncontrado.Cedula} • {clienteEncontrado.Telefono}";
                PanelClienteVisible = true;
            }
            else
            {
                PanelClienteVisible = false;
                if (mostrarDialogoSiNoExiste)
                {
                    _dialogService.Informar(
                        $"No se encontró ningún cliente con la cédula {Cedula}.",
                        "Cliente no encontrado");
                }
            }

            OcultarSugerenciasCedula();
        }

        private void ActualizarSugerenciasCedula()
        {
            CedulaSugeridaSeleccionada = null;

            if (string.IsNullOrWhiteSpace(Cedula))
            {
                OcultarSugerenciasCedula();
                return;
            }

            var coincidencias = _db.ObtenerCedulasPorPrefijo(Cedula, 8);

            CedulasSugeridas = new ObservableCollection<string>(coincidencias);
            MostrarSugerenciasCedula = coincidencias.Count > 0;
        }

        private void OcultarSugerenciasCedula()
        {
            CedulasSugeridas = new ObservableCollection<string>();
            MostrarSugerenciasCedula = false;
        }

        public void ConfirmarReserva(DateTime fechaEntrada, DateTime fechaSalida)
        {
            if (SalaSeleccionada == null)
            {
                _dialogService.Informar("Selecciona una sala para crear la reserva.", "Campo requerido");
                return;
            }

            var cliente = _db.BuscarClientePorCedula(Cedula);
            if (cliente == null)
            {
                _dialogService.Informar("Debes seleccionar un cliente válido.", "Cliente requerido");
                return;
            }

            if (fechaSalida <= fechaEntrada)
            {
                _dialogService.Informar("La fecha y hora de salida deben ser posteriores a la entrada.", "Fechas inválidas");
                return;
            }

            string salaTexto = SalaSeleccionada.NombreConCapacidad;
            string fechasTexto = $"{fechaEntrada:dd/MM/yyyy hh:mm tt} -> {fechaSalida:dd/MM/yyyy hh:mm tt}";
            string clienteTexto = $"{cliente.NombreCompleto} • {cliente.Cedula}";

            bool confirmado = _dialogService.ConfirmarReserva(salaTexto, fechasTexto, clienteTexto);
            if (!confirmado) return;

            if (_db.AgregarReserva(SalaSeleccionada.Id, cliente.Id, fechaEntrada, fechaSalida, out string error))
            {
                _dialogService.MostrarReservaExitosa(salaTexto, fechasTexto, clienteTexto);
                NavegaAlInicio?.Invoke();
            }
            else
            {
                _dialogService.Informar($"No se pudo guardar la reserva.\n{error}", "Error de guardado");
            }
        }

        
    }
}