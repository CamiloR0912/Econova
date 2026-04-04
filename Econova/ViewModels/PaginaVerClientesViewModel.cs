using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Econova.Core;
using Econova.Models;
using Econova.Services;

namespace Econova.ViewModels
{
    public class PaginaVerClientesViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        private List<Cliente> _todosClientes = new List<Cliente>();
        private ObservableCollection<Cliente> _clientesFiltrados;
        private string _textoBusqueda;
        private string _contadorTexto;
        private string _subtituloTexto;

        public ObservableCollection<Cliente> ClientesFiltrados
        {
            get => _clientesFiltrados;
            set => SetProperty(ref _clientesFiltrados, value);
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

        public PaginaVerClientesViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            EliminarCommand = new RelayCommand(o => Eliminar(o));
            CargarClientesEjemplo();
        }

        private void Eliminar(object parameter)
        {
            if (parameter is int id)
            {
                var c = _todosClientes.FirstOrDefault(x => x.Id == id);
                if (c == null) return;

                bool confirmado = _dialogService.Confirmar(
                    $"¿Estás seguro de que deseas eliminar este cliente?\n\n" +
                    $"  Nombre:   {c.NombreCompleto}\n" +
                    $"  Cédula:   {c.Cedula}\n" +
                    $"  Email:    {c.Email}\n\n" +
                    $"Esta acción no se puede deshacer.",
                    "Confirmar eliminación");

                if (confirmado)
                {
                    _todosClientes.Remove(c);

                    for (int i = 0; i < _todosClientes.Count; i++)
                        _todosClientes[i].Numero = i + 1;

                    Filtrar();

                    _dialogService.Informar(
                        "Cliente eliminado exitosamente.",
                        "Eliminación confirmada");
                }
            }
        }

        private void CargarClientesEjemplo()
        {
            _todosClientes = new List<Cliente>
            {
                new Cliente { Id=1, Nombres="Juan",   Apellidos="Pérez",  Cedula="1234567890", Email="juan@email.com",   Telefono="3101234567", Direccion="Calle 1 # 2-3" },
                new Cliente { Id=2, Nombres="María",  Apellidos="López",  Cedula="0987654321", Email="maria@email.com",  Telefono="3207654321", Direccion="Carrera 4 # 5-6" },
                new Cliente { Id=3, Nombres="Carlos", Apellidos="Ruiz",   Cedula="1122334455", Email="carlos@email.com", Telefono="3151122334", Direccion="Av 7 # 8-9" },
                new Cliente { Id=4, Nombres="Ana",    Apellidos="Torres", Cedula="5566778899", Email="ana@email.com",    Telefono="3005566778", Direccion="Calle 10 # 11-12" },
                new Cliente { Id=5, Nombres="Luis",   Apellidos="Gómez",  Cedula="9988776655", Email="luis@email.com",   Telefono="3119988776", Direccion="Carrera 13 # 14-15" },
            };

            for (int i = 0; i < _todosClientes.Count; i++)
                _todosClientes[i].Numero = i + 1;

            ClientesFiltrados = new ObservableCollection<Cliente>(_todosClientes);
            ActualizarTextos();
        }

        private void Filtrar()
        {
            string q = TextoBusqueda?.Trim().ToLower() ?? "";

            var filtrados = string.IsNullOrEmpty(q)
                ? _todosClientes
                : _todosClientes.Where(c =>
                    c.NombreCompleto.ToLower().Contains(q) ||
                    c.Cedula.Contains(q) ||
                    c.Email.ToLower().Contains(q) ||
                    c.Telefono.Contains(q)).ToList();

            ClientesFiltrados = new ObservableCollection<Cliente>(filtrados);
            ActualizarTextos();
        }

        private void ActualizarTextos()
        {
            int total = ClientesFiltrados.Count;
            ContadorTexto = total == 1 ? "1 cliente" : $"{total} clientes";
            SubtituloTexto = total == _todosClientes.Count
                ? "Todos los clientes registrados"
                : $"{total} resultado(s) encontrado(s)";
        }
    }
}