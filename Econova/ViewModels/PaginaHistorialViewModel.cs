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
    /// RF-08: ViewModel para la consulta del historial de visitas filtrado por fecha seleccionada.
    /// </summary>
    public class PaginaHistorialViewModel : ObservableObject
    {
        private readonly SqliteDataService _db = SqliteDataService.Instance;
        private List<Reserva> _todasVisitas = new List<Reserva>();
        private ObservableCollection<Reserva> _visitasFiltradas;
        private DateTime? _fechaSeleccionada;
        private string _contadorTexto;

        public ObservableCollection<Reserva> VisitasFiltradas
        {
            get => _visitasFiltradas;
            set => SetProperty(ref _visitasFiltradas, value);
        }

        public DateTime? FechaSeleccionada
        {
            get => _fechaSeleccionada;
            set
            {
                if (SetProperty(ref _fechaSeleccionada, value))
                    Filtrar();
            }
        }

        public string ContadorTexto
        {
            get => _contadorTexto;
            set => SetProperty(ref _contadorTexto, value);
        }

        public PaginaHistorialViewModel()
        {
            CargarHistorial();
        }

        private void CargarHistorial()
        {
            _todasVisitas = _db.ObtenerHistorialVisitas();
            // Inicializar con la fecha de hoy
            FechaSeleccionada = DateTime.Today;
        }

        private void Filtrar()
        {
            var filtradas = _todasVisitas.AsEnumerable();

            // Filtrar por la fecha seleccionada
            if (FechaSeleccionada.HasValue)
            {
                filtradas = filtradas.Where(v => v.FechaEntradaDt.Date == FechaSeleccionada.Value.Date);
            }

            var resultado = filtradas.ToList();
            // Re-numerar
            for (int i = 0; i < resultado.Count; i++)
                resultado[i].Numero = i + 1;

            VisitasFiltradas = new ObservableCollection<Reserva>(resultado);
            ContadorTexto = resultado.Count == 1 ? "1 visita" : $"{resultado.Count} visitas";
        }
    }
}
