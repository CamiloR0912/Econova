using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Econova.Core;
using Econova.Models;

namespace Econova.ViewModels
{
    public class PaginaVerReservasViewModel : ObservableObject
    {
        private DateTime _diaActual = DateTime.Today;
        private List<Reserva> _todasReservas = new List<Reserva>();
        private ObservableCollection<Reserva> _reservasDelDia;
        private string _fechaDiaTexto;
        private string _diaSemanaTexto;
        private string _subtituloTexto;
        private string _contadorTexto;

        public DateTime DiaActual
        {
            get => _diaActual;
            set
            {
                if (SetProperty(ref _diaActual, value))
                {
                    ActualizarDia();
                }
            }
        }

        public ObservableCollection<Reserva> ReservasDelDia
        {
            get => _reservasDelDia;
            set => SetProperty(ref _reservasDelDia, value);
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

        private ObservableCollection<Reserva> _reservasFiltradas;
        private string _textoBusqueda;
        private Visibility _calendarioVisibility = Visibility.Visible;
        private Visibility _todasReservasVisibility = Visibility.Collapsed;

        public ObservableCollection<Reserva> ReservasFiltradas
        {
            get => _reservasFiltradas;
            set => SetProperty(ref _reservasFiltradas, value);
        }

        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (SetProperty(ref _textoBusqueda, value))
                {
                    Filtrar();
                }
            }
        }

        public Visibility CalendarioVisibility
        {
            get => _calendarioVisibility;
            set => SetProperty(ref _calendarioVisibility, value);
        }

        public Visibility TodasReservasVisibility
        {
            get => _todasReservasVisibility;
            set => SetProperty(ref _todasReservasVisibility, value);
        }

        public ICommand VerCalendarioCommand { get; }
        public ICommand VerTodasCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand CancelarCommand { get; }

        public PaginaVerReservasViewModel()
        {
            VerCalendarioCommand = new RelayCommand(o => { CalendarioVisibility = Visibility.Visible; TodasReservasVisibility = Visibility.Collapsed; });
            VerTodasCommand = new RelayCommand(o => { CalendarioVisibility = Visibility.Collapsed; TodasReservasVisibility = Visibility.Visible; });
            EditarCommand = new RelayCommand(o => Editar(o));
            CancelarCommand = new RelayCommand(o => Cancelar(o));
            
            DiaAnteriorCommand = new RelayCommand(o => DiaActual = DiaActual.AddDays(-1));
            DiaSiguienteCommand = new RelayCommand(o => DiaActual = DiaActual.AddDays(1));
            HoyCommand = new RelayCommand(o => DiaActual = DateTime.Today);

            CargarReservasEjemplo();
            Filtrar();
            ActualizarDia();
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
            if (TodasReservasVisibility == Visibility.Visible)
            {
                int total = ReservasFiltradas.Count;
                ContadorTexto = total == 1 ? "1 reserva" : $"{total} reservas";
            }
            else
            {
                int total = ReservasDelDia?.Count ?? 0;
                ContadorTexto = total == 1 ? "1 reserva" : $"{total} reservas";
            }
        }

        private void Editar(object parameter)
        {
            if (parameter is int id)
            {
                var r = _todasReservas.FirstOrDefault(x => x.Id == id);
                if (r != null)
                {
                    MessageBox.Show($"Abriendo editor para la reserva #{r.Numero} de {r.Cliente}", "Editar Reserva", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Cancelar(object parameter)
        {
            if (parameter is int id)
            {
                var r = _todasReservas.FirstOrDefault(x => x.Id == id);
                if (r != null)
                {
                    var result = MessageBox.Show($"¿Estás seguro de que deseas cancelar la reserva #{r.Numero}?", "Confirmar Cancelación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        _todasReservas.Remove(r);
                        Filtrar();
                        ActualizarDia();
                    }
                }
            }
        }

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
        }

        private void ActualizarDia()
        {
            string[] diasSemana = { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" };
            string[] meses = { "enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre" };

            FechaDiaTexto = $"{DiaActual.Day} de {meses[DiaActual.Month - 1]} de {DiaActual.Year}";
            DiaSemanaTexto = diasSemana[(int)DiaActual.DayOfWeek];
            SubtituloTexto = DiaActual.Date == DateTime.Today ? "Hoy" :
                             DiaActual.Date == DateTime.Today.AddDays(1) ? "Mañana" :
                             DiaActual.ToString("dd/MM/yyyy");

            var reservas = _todasReservas
                .Where(r => r.FechaEntradaDt.Date == DiaActual.Date)
                .ToList();
            
            ReservasDelDia = new ObservableCollection<Reserva>(reservas);
            
            int total = ReservasDelDia.Count;
            ContadorTexto = total == 1 ? "1 reserva" : $"{total} reservas";
        }
    }
}
