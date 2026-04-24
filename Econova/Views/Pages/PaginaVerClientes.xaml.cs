using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Econova.ViewModels;
using Econova.Models;
using Econova.Views.Windows;
using Econova.Views.Services;

namespace Econova.Views.Pages
{
    public partial class PaginaVerClientes : Page
    {
        public PaginaVerClientes()
        {
            InitializeComponent();
            DataContext = new PaginaVerClientesViewModel(new WpfDialogService());
        }

        private void BtnNuevoCliente_Click(object sender, RoutedEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;
            main?.NavigarACrearCliente();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            var vm = DataContext as PaginaVerClientesViewModel;
            var c = vm?.ClientesFiltrados.FirstOrDefault(x => x.Id == id);
            if (c == null) return;

            var ventana = new VentanaEditarCliente(c);
            ventana.Owner = Window.GetWindow(this);

            if (ventana.ShowDialog() == true)
            {
                vm.CargarClientes(); // ← agrega esta línea

                var confirmacion = new ConfirmacionClienteWindow(
                    c.NombreCompleto, c.Cedula, c.Telefono, c.Email, c.Direccion)
                {
                    Owner = Window.GetWindow(this)
                };
                confirmacion.ShowDialog();
            }
        }
    }
}