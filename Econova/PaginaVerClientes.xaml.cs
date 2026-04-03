using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Econova
{
    public class Cliente
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string NombreCompleto { get; set; }
        public string Inicial { get; set; }
        public string Cedula { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
    }

    public partial class PaginaVerClientes : Page
    {
        private List<Cliente> _todosClientes = new List<Cliente>();
        private List<Cliente> _clientesFiltrados = new List<Cliente>();

        public PaginaVerClientes()
        {
            InitializeComponent();
            CargarClientesEjemplo();
        }

        // ── Datos de ejemplo — reemplaza con tu consulta a base de datos ──
        private void CargarClientesEjemplo()
        {
            _todosClientes = new List<Cliente>
            {
                new Cliente { Id=1, Nombres="Juan",    Apellidos="Pérez",    Cedula="1234567890", Email="juan@email.com",   Telefono="3101234567", Direccion="Calle 1 # 2-3" },
                new Cliente { Id=2, Nombres="María",   Apellidos="López",    Cedula="0987654321", Email="maria@email.com",  Telefono="3207654321", Direccion="Carrera 4 # 5-6" },
                new Cliente { Id=3, Nombres="Carlos",  Apellidos="Ruiz",     Cedula="1122334455", Email="carlos@email.com", Telefono="3151122334", Direccion="Av 7 # 8-9" },
                new Cliente { Id=4, Nombres="Ana",     Apellidos="Torres",   Cedula="5566778899", Email="ana@email.com",    Telefono="3005566778", Direccion="Calle 10 # 11-12" },
                new Cliente { Id=5, Nombres="Luis",    Apellidos="Gómez",    Cedula="9988776655", Email="luis@email.com",   Telefono="3119988776", Direccion="Carrera 13 # 14-15" },
            };

            NumerarYCompletar();
            _clientesFiltrados = new List<Cliente>(_todosClientes);
            ActualizarTabla();
        }

        // Asigna número, inicial y nombre completo
        private void NumerarYCompletar()
        {
            for (int i = 0; i < _todosClientes.Count; i++)
            {
                var c = _todosClientes[i];
                c.Numero = i + 1;
                c.NombreCompleto = $"{c.Nombres} {c.Apellidos}";
                c.Inicial = c.Nombres.Length > 0
                                   ? c.Nombres[0].ToString().ToUpper()
                                   : "?";
            }
        }

        private void ActualizarTabla()
        {
            int total = _clientesFiltrados.Count;
            TxtContador.Text = total == 1 ? "1 cliente" : $"{total} clientes";
            TxtSubtitulo.Text = total == _todosClientes.Count
                ? "Todos los clientes registrados"
                : $"{total} resultado(s) encontrado(s)";

            ListaClientes.ItemsSource = null;
            ListaClientes.ItemsSource = _clientesFiltrados;
        }

        // ── Búsqueda ──
        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string q = TxtBuscar.Text.Trim().ToLower();

            _clientesFiltrados = string.IsNullOrEmpty(q)
                ? new List<Cliente>(_todosClientes)
                : _todosClientes.Where(c =>
                    c.NombreCompleto.ToLower().Contains(q) ||
                    c.Cedula.Contains(q) ||
                    c.Email.ToLower().Contains(q) ||
                    c.Telefono.Contains(q)).ToList();

            ActualizarTabla();
        }

        // ── Nuevo cliente ──
        private void BtnNuevoCliente_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new PaginaCrearCliente());
        }

        // ── Editar cliente ──
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            var c = _todosClientes.FirstOrDefault(x => x.Id == id);
            if (c == null) return;

            // Abre ventana de edición con los datos del cliente
            var ventana = new VentanaEditarCliente(c);
            ventana.Owner = Window.GetWindow(this);

            if (ventana.ShowDialog() == true)
            {
                // Actualiza los datos en la lista
                c.Nombres = ventana.ClienteEditado.Nombres;
                c.Apellidos = ventana.ClienteEditado.Apellidos;
                c.Email = ventana.ClienteEditado.Email;
                c.Telefono = ventana.ClienteEditado.Telefono;
                c.Direccion = ventana.ClienteEditado.Direccion;
                c.NombreCompleto = $"{c.Nombres} {c.Apellidos}";
                c.Inicial = c.Nombres[0].ToString().ToUpper();

                // ── Aquí actualizarás en tu base de datos ──

                _clientesFiltrados = new List<Cliente>(_todosClientes);
                ActualizarTabla();

                MessageBox.Show("Cliente actualizado exitosamente.",
                    "Actualización exitosa",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ── Eliminar cliente ──
        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            var c = _todosClientes.FirstOrDefault(x => x.Id == id);
            if (c == null) return;

            var resultado = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar este cliente?\n\n" +
                $"  Nombre:   {c.NombreCompleto}\n" +
                $"  Cédula:   {c.Cedula}\n" +
                $"  Email:    {c.Email}\n\n" +
                $"Esta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                // ── Aquí eliminarás de tu base de datos ──
                _todosClientes.Remove(c);
                _clientesFiltrados.Remove(c);

                // Renumerar
                for (int i = 0; i < _todosClientes.Count; i++)
                    _todosClientes[i].Numero = i + 1;

                ActualizarTabla();

                MessageBox.Show("Cliente eliminado exitosamente.",
                    "Eliminación confirmada",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}