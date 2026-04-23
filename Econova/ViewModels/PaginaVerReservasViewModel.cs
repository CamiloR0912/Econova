using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Econova.Core;
using Econova.Infrastructure;
using Econova.Models;
using Econova.Services;

namespace Econova.ViewModels
{
    public class PaginaVerReservasViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        private readonly SqliteDataService _db = SqliteDataService.Instance;

        private DateTime _diaActual = DateTime.Today;
        private List<Reserva> _todasReservas = new List<Reserva>();
        private ObservableCollection<Reserva> _reservasDelDia;
        private ObservableCollection<Reserva> _reservasFiltradas;
        private string _fechaDiaTexto;
        private string _diaSemanaTexto;
        private string _subtituloTexto;
        private string _contadorTexto;
        private string _textoBusqueda;
        private bool _mostrarCalendario = true;

        public DateTime DiaActual
        {
            get => _diaActual;
            set
            {
                if (SetProperty(ref _diaActual, value))
                    ActualizarDia();
            }
        }

        public ObservableCollection<Reserva> ReservasDelDia
        {
            get => _reservasDelDia;
            set => SetProperty(ref _reservasDelDia, value);
        }

        public ObservableCollection<Reserva> ReservasFiltradas
        {
            get => _reservasFiltradas;
            set => SetProperty(ref _reservasFiltradas, value);
        }

        public string FechaDiaTexto
        {
            get => _fechaDiaTexto;
            set => SetProperty(ref _fechaDiaTexto, value);
        }

        public string DiaSemanaTexto
        {
            get => _diaSemanaTexto;
            set => SetProperty(ref _diaSemanaTexto, value);
        }

        public string SubtituloTexto
        {
            get => _subtituloTexto;
            set => SetProperty(ref _subtituloTexto, value);
        }

        public string ContadorTexto
        {
            get => _contadorTexto;
            set => SetProperty(ref _contadorTexto, value);
        }

        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (SetProperty(ref _textoBusqueda, value))
                    Filtrar();
            }
        }

        public bool MostrarCalendario
        {
            get => _mostrarCalendario;
            set
            {
                if (SetProperty(ref _mostrarCalendario, value))
                {
                    OnPropertyChanged(nameof(MostrarTodas));
                    ActualizarTextos();
                }
            }
        }

        public bool MostrarTodas => !MostrarCalendario;

        public ICommand VerCalendarioCommand { get; }
        public ICommand VerTodasCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand DiaAnteriorCommand { get; }
        public ICommand DiaSiguienteCommand { get; }
        public ICommand HoyCommand { get; }

        public PaginaVerReservasViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            VerCalendarioCommand = new RelayCommand(o => MostrarCalendario = true);
            VerTodasCommand = new RelayCommand(o => MostrarCalendario = false);
            EditarCommand = new RelayCommand(o => Editar(o));
            CancelarCommand = new RelayCommand(o => Cancelar(o));
            DiaAnteriorCommand = new RelayCommand(o => DiaActual = DiaActual.AddDays(-1));
            DiaSiguienteCommand = new RelayCommand(o => DiaActual = DiaActual.AddDays(1));
            HoyCommand = new RelayCommand(o => DiaActual = DateTime.Today);

            CargarReservas();
            Filtrar();
            ActualizarDia();
        }

        private void Editar(object parameter)
        {
            if (parameter is int id)
            {
                var r = _todasReservas.FirstOrDefault(x => x.Id == id);
                if (r != null)
                {
                    bool guardado = _dialogService.EditarReserva(r);
                    if (guardado)
                    {
                        Filtrar();
                        ActualizarDia();
                    }
                }
            }
        }

        private void Cancelar(object parameter)
        {
            if (parameter is int id)
            {
                var r = _todasReservas.FirstOrDefault(x => x.Id == id);
                if (r == null) return;

                bool confirmado = _dialogService.ConfirmarEliminarReserva(
                    r.Sala, r.Cliente, r.Cedula,
                    $"{r.FechaEntrada} {r.HoraEntrada}");

                if (confirmado)
                {
                    _db.EliminarReserva(r.Id);
                    CargarReservas();
                    Filtrar();
                    ActualizarDia();
                }
            }
        }

        private void Filtrar()
        {
            string q = TextoBusqueda?.Trim().ToLower() ?? "";

            var filtradas = string.IsNullOrEmpty(q)
                ? _todasReservas
                : _todasReservas.Where(r =>
                    r.Sala.ToLower().Contains(q) ||
                    r.Cliente.ToLower().Contains(q) ||
                    r.Cedula.Contains(q)).ToList();

            ReservasFiltradas = new ObservableCollection<Reserva>(filtradas);
            ActualizarTextos();
        }

        private void ActualizarTextos()
        {
            int total = MostrarCalendario
                ? (ReservasDelDia?.Count ?? 0)
                : (ReservasFiltradas?.Count ?? 0);

            ContadorTexto = total == 1 ? "1 reserva" : $"{total} reservas";
        }

        private void ActualizarDia()
        {
            string[] diasSemana = { "Domingo", "Lunes", "Martes", "Miércoles",
                                    "Jueves", "Viernes", "Sábado" };
            string[] meses = { "enero", "febrero", "marzo", "abril", "mayo",
                                    "junio", "julio", "agosto", "septiembre",
                                    "octubre", "noviembre", "diciembre" };

            FechaDiaTexto = $"{DiaActual.Day} de {meses[DiaActual.Month - 1]} de {DiaActual.Year}";
            DiaSemanaTexto = diasSemana[(int)DiaActual.DayOfWeek];
            SubtituloTexto = DiaActual.Date == DateTime.Today ? "Hoy" :
                             DiaActual.Date == DateTime.Today.AddDays(1) ? "Mañana" :
                             DiaActual.ToString("dd/MM/yyyy");

            var reservas = _todasReservas
                .Where(r => r.FechaEntradaDt.Date == DiaActual.Date)
                .ToList();

            ReservasDelDia = new ObservableCollection<Reserva>(reservas);
            ActualizarTextos();
        }

        private void CargarReservas()
        {
            _todasReservas = _db.ObtenerReservas();
        }
    }
}