using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Econova.Core;
using Econova.Infrastructure;
using Econova.Models;

namespace Econova.ViewModels
{
    /// <summary>
    /// RF-07: ViewModel para el panel interactivo de salas con estado Disponible/Ocupada.
    /// </summary>
    public class PaginaPanelSalasViewModel : ObservableObject
    {
        private readonly SqliteDataService _db = SqliteDataService.Instance;
        private ObservableCollection<SalaConEstado> _salas;
        private SalaConEstado _salaSeleccionada;
        private ObservableCollection<Reserva> _reservasSalaSeleccionada;
        private bool _detalleSalaVisible;
        private string _contadorTexto;
        private string _resumenTexto;

        public ObservableCollection<SalaConEstado> Salas
        {
            get => _salas;
            set => SetProperty(ref _salas, value);
        }

        public SalaConEstado SalaSeleccionada
        {
            get => _salaSeleccionada;
            set
            {
                if (SetProperty(ref _salaSeleccionada, value))
                {
                    CargarDetalleReservas();
                }
            }
        }

        public ObservableCollection<Reserva> ReservasSalaSeleccionada
        {
            get => _reservasSalaSeleccionada;
            set => SetProperty(ref _reservasSalaSeleccionada, value);
        }

        public bool DetalleSalaVisible
        {
            get => _detalleSalaVisible;
            set => SetProperty(ref _detalleSalaVisible, value);
        }

        public string ContadorTexto
        {
            get => _contadorTexto;
            set => SetProperty(ref _contadorTexto, value);
        }

        public string ResumenTexto
        {
            get => _resumenTexto;
            set => SetProperty(ref _resumenTexto, value);
        }

        public ICommand SeleccionarSalaCommand { get; }
        public ICommand CerrarDetalleCommand { get; }
        public ICommand ActualizarCommand { get; }

        public PaginaPanelSalasViewModel()
        {
            SeleccionarSalaCommand = new RelayCommand(o => SeleccionarSala(o));
            CerrarDetalleCommand = new RelayCommand(o => CerrarDetalle());
            ActualizarCommand = new RelayCommand(o => CargarSalas());
            CargarSalas();
        }

        public void CargarSalas()
        {
            var salasDb = _db.ObtenerSalas();
            var salasConEstado = salasDb.Select(s =>
            {
                bool ocupada = _db.SalaOcupadaAhora(s.Id);
                var reservas = _db.ObtenerReservasActivasPorSala(s.Id);
                return new SalaConEstado
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Capacidad = s.Capacidad,
                    Ocupada = ocupada,
                    ReservasActivas = reservas
                };
            }).ToList();

            Salas = new ObservableCollection<SalaConEstado>(salasConEstado);

            int total = salasConEstado.Count;
            int disponibles = salasConEstado.Count(s => !s.Ocupada);
            int ocupadas = salasConEstado.Count(s => s.Ocupada);

            ContadorTexto = $"{total} salas";
            ResumenTexto = $"{disponibles} disponibles • {ocupadas} ocupadas";

            // Si había una sala seleccionada, refrescar detalle
            if (SalaSeleccionada != null)
            {
                var actualizada = salasConEstado.FirstOrDefault(s => s.Id == SalaSeleccionada.Id);
                if (actualizada != null)
                {
                    SalaSeleccionada = actualizada;
                }
                else
                {
                    CerrarDetalle();
                }
            }
        }

        private void SeleccionarSala(object parameter)
        {
            if (parameter is int id)
            {
                var sala = Salas?.FirstOrDefault(s => s.Id == id);
                if (sala != null)
                {
                    SalaSeleccionada = sala;
                    DetalleSalaVisible = true;
                }
            }
        }

        private void CargarDetalleReservas()
        {
            if (SalaSeleccionada == null)
            {
                ReservasSalaSeleccionada = new ObservableCollection<Reserva>();
                return;
            }
            ReservasSalaSeleccionada = new ObservableCollection<Reserva>(SalaSeleccionada.ReservasActivas);
        }

        private void CerrarDetalle()
        {
            DetalleSalaVisible = false;
            SalaSeleccionada = null;
            ReservasSalaSeleccionada = new ObservableCollection<Reserva>();
        }
    }
}
