using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Econova.ViewModels;
using Econova.Models;
using Econova.Views.Windows;
using Econova.Views.Services; // 👈

namespace Econova.Views.Pages
{
    public partial class PaginaVerClientes : Page
    {
        public PaginaVerClientes()
        {
            InitializeComponent();
            DataContext = new PaginaVerClientesViewModel(new WpfDialogService()); // 👈
        }

        private void BtnNuevoCliente_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new PaginaCrearCliente());
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
                new WpfDialogService().Informar(
                    "Cliente actualizado exitosamente.",
                    "Actualización exitosa");
            }
        }
    }
}