using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Econova.Core;
using Econova.Infrastructure;
using Econova.Models;

namespace Econova.ViewModels
{
    /// <summary>
    /// RF-08: ViewModel para la consulta del historial de visitas con filtros por sala, cliente/cédula y fechas.
    /// </summary>
    public class PaginaHistorialViewModel : ObservableObject
    {
        private readonly SqliteDataService _db = SqliteDataService.Instance;
        private List<Reserva> _todasVisitas = new List<Reserva>();
        private ObservableCollection<Reserva> _visitasFiltradas;
        private string _textoBusqueda;
        private string _filtroSala;
        private DateTime? _fechaDesde;
        private DateTime? _fechaHasta;
        private string _contadorTexto;
        private ObservableCollection<string> _salasDisponibles;

        public ObservableCollection<Reserva> VisitasFiltradas
        {
            get => _visitasFiltradas;
            set => SetProperty(ref _visitasFiltradas, value);
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

        public string FiltroSala
        {
            get => _filtroSala;
            set
            {
                if (SetProperty(ref _filtroSala, value))
                    Filtrar();
            }
        }

        public DateTime? FechaDesde
        {
            get => _fechaDesde;
            set
            {
                if (SetProperty(ref _fechaDesde, value))
                    Filtrar();
            }
        }

        public DateTime? FechaHasta
        {
            get => _fechaHasta;
            set
            {
                if (SetProperty(ref _fechaHasta, value))
                    Filtrar();
            }
        }

        public string ContadorTexto
        {
            get => _contadorTexto;
            set => SetProperty(ref _contadorTexto, value);
        }

        public ObservableCollection<string> SalasDisponibles
        {
            get => _salasDisponibles;
            set => SetProperty(ref _salasDisponibles, value);
        }

        public ICommand LimpiarFiltrosCommand { get; }
        public ICommand ActualizarCommand { get; }

        public PaginaHistorialViewModel()
        {
            LimpiarFiltrosCommand = new RelayCommand(o => LimpiarFiltros());
            ActualizarCommand = new RelayCommand(o => CargarHistorial());
            CargarHistorial();
        }

        private void CargarHistorial()
        {
            _todasVisitas = _db.ObtenerHistorialVisitas();

            // Obtener lista de salas únicas para el filtro
            var salas = _todasVisitas.Select(v => v.Sala).Distinct().OrderBy(s => s).ToList();
            salas.Insert(0, "Todas las salas");
            SalasDisponibles = new ObservableCollection<string>(salas);

            if (string.IsNullOrEmpty(FiltroSala))
                FiltroSala = "Todas las salas";

            Filtrar();
        }

        private void Filtrar()
        {
            string q = TextoBusqueda?.Trim().ToLower() ?? "";
            var filtradas = _todasVisitas.AsEnumerable();

            // Filtrar por texto (cliente o cédula)
            if (!string.IsNullOrEmpty(q))
            {
                filtradas = filtradas.Where(v =>
                    v.Cliente.ToLower().Contains(q) ||
                    v.Cedula.Contains(q));
            }

            // Filtrar por sala
            if (!string.IsNullOrEmpty(FiltroSala) && FiltroSala != "Todas las salas")
            {
                filtradas = filtradas.Where(v => v.Sala == FiltroSala);
            }

            // Filtrar por fecha desde
            if (FechaDesde.HasValue)
            {
                filtradas = filtradas.Where(v => v.FechaEntradaDt.Date >= FechaDesde.Value.Date);
            }

            // Filtrar por fecha hasta
            if (FechaHasta.HasValue)
            {
                filtradas = filtradas.Where(v => v.FechaEntradaDt.Date <= FechaHasta.Value.Date);
            }

            var resultado = filtradas.ToList();
            // Re-numerar
            for (int i = 0; i < resultado.Count; i++)
                resultado[i].Numero = i + 1;

            VisitasFiltradas = new ObservableCollection<Reserva>(resultado);
            ContadorTexto = resultado.Count == 1 ? "1 visita" : $"{resultado.Count} visitas";
        }

        private void LimpiarFiltros()
        {
            TextoBusqueda = string.Empty;
            FiltroSala = "Todas las salas";
            FechaDesde = null;
            FechaHasta = null;
        }
    }
}
