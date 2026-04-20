using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Econova.Core;
using Econova.Models;
using Econova.Services;

namespace Econova.ViewModels
{
    public class PaginaVerSalasViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
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
            CargarSalasEjemplo();
        }

        private void Eliminar(object parameter)
        {
            if (parameter is int id)
            {
                var s = _todasSalas.FirstOrDefault(x => x.Id == id);
                if (s == null) return;

                bool confirmado = _dialogService.Confirmar(
                    $"¿Estás seguro de que deseas eliminar esta sala?\n\n" +
                    $"  Nombre:      {s.Nombre}\n" +
                    $"  Capacidad:   {s.Capacidad} personas\n\n" +
                    $"Esta acción no se puede deshacer.",
                    "Confirmar eliminación");

                if (confirmado)
                {
                    _todasSalas.Remove(s);

                    for (int i = 0; i < _todasSalas.Count; i++)
                        _todasSalas[i].Numero = i + 1;

                    Filtrar();

                    _dialogService.Informar(
                        "Sala eliminada exitosamente.",
                        "Eliminación confirmada");
                }
            }
        }

        private void CargarSalasEjemplo()
        {
            _todasSalas = new List<Sala>
            {
                new Sala { Id=1, Nombre="Sala A", Capacidad=20 },
                new Sala { Id=2, Nombre="Sala B", Capacidad=15 },
                new Sala { Id=3, Nombre="Sala C", Capacidad=30 },
                new Sala { Id=4, Nombre="Auditorio", Capacidad=100 },
                new Sala { Id=5, Nombre="Sala de Juntas", Capacidad=10 },
            };

            for (int i = 0; i < _todasSalas.Count; i++)
                _todasSalas[i].Numero = i + 1;

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

        public void AgregarNuevaSala(string nombre, int capacidad)
        {
            int nuevoId = _todasSalas.Any() ? _todasSalas.Max(s => s.Id) + 1 : 1;
            var nuevaSala = new Sala
            {
                Id = nuevoId,
                Nombre = nombre,
                Capacidad = capacidad,
                Numero = _todasSalas.Count + 1
            };

            _todasSalas.Add(nuevaSala);
            Filtrar();
        }
    }
}
