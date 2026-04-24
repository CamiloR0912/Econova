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
    public class PaginaVerClientesViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        private readonly SqliteDataService _db = SqliteDataService.Instance;
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
            CargarClientes();
        }

        private void Eliminar(object parameter)
        {
            if (parameter is int id)
            {
                var c = _todosClientes.FirstOrDefault(x => x.Id == id);
                if (c == null) return;

                var ventana = new Econova.Views.Windows.ConfirmacionEliminarClienteWindow(
                    c.NombreCompleto, c.Cedula, c.Email)
                {
                    Owner = System.Windows.Application.Current.MainWindow
                };
                ventana.ShowDialog();

                if (ventana.Confirmado)
                {
                    _db.EliminarCliente(c.Id);
                    CargarClientes();
                }
            }
        }

        public void CargarClientes()
        {
            _todosClientes = _db.ObtenerClientes();
            Filtrar();
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