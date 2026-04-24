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
    public class PaginaVerSalasViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        private readonly SqliteDataService _db = SqliteDataService.Instance;
        private List<Sala> _todasSalas = new List<Sala>();
        private ObservableCollection<Sala> _salasFiltradas;
        private string _textoBusqueda;
        private string _contadorTexto;
        private string _subtituloTexto;

        public ObservableCollection<Sala> SalasFiltradas
        {
            get => _salasFiltradas;
            set => SetProperty(ref _salasFiltradas, value);
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

        public string ContadorTexto
        {
            get => _contadorTexto;
            set => SetProperty(ref _contadorTexto, value);
        }

        public string SubtituloTexto
        {
            get => _subtituloTexto;
            set => SetProperty(ref _subtituloTexto, value);
        }

        public ICommand EliminarCommand { get; }

        public PaginaVerSalasViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            EliminarCommand = new RelayCommand(o => Eliminar(o));
            CargarSalas();
        }

        private void Eliminar(object parameter)
        {
            if (parameter is int id)
            {
                var s = _todasSalas.FirstOrDefault(x => x.Id == id);
                if (s == null) return;

                bool confirmado = _dialogService.ConfirmarEliminarSala(s.Nombre, s.Capacidad);

                if (confirmado)
                {
                    bool exito = _db.EliminarSala(s.Id);
                    if (exito)
                        CargarSalas();
                    else
                        _dialogService.Informar("No se pudo eliminar la sala.", "Error");
                }
            }
        }

        private void CargarSalas()
        {
            _todasSalas = _db.ObtenerSalas();
            Filtrar();
        }

        private void Filtrar()
        {
            var filtrados = string.IsNullOrWhiteSpace(TextoBusqueda)
                ? _todasSalas
                : _todasSalas.Where(s =>
                    s.Nombre.ToLower().Contains(TextoBusqueda.ToLower())).ToList();

            SalasFiltradas = new ObservableCollection<Sala>(filtrados);
            ContadorTexto = $"{filtrados.Count} salas encontradas";
            SubtituloTexto = "Gestión y administración de los espacios disponibles";
        }

        public bool AgregarNuevaSala(string nombre, int capacidad)
        {
            if (_db.AgregarSala(nombre, capacidad, out string error))
            {
                CargarSalas();
                return true;
            }
            
            _dialogService.Informar($"No se pudo guardar la sala.\n{error}", "Error de guardado");
            return false;
        }
    }
}
