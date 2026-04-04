using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Econova.ViewModels;
using Econova.Models;
using Econova.Views.Windows;

namespace Econova.Views.Pages
{
    public partial class PaginaVerClientes : Page
    {
        public PaginaVerClientes()
        {
            InitializeComponent();
            DataContext = new PaginaVerClientesViewModel();
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

            // Abre ventana de edición con los datos del cliente
            var ventana = new VentanaEditarCliente(c);
            ventana.Owner = Window.GetWindow(this);

            if (ventana.ShowDialog() == true)
            {
                // Note: The object c is already updated by the dialog if it modifies the same object.
                // However, we should ensure the ViewModel is aware of changes.
                // In a full MVVM approach, this would be handled differently.
                
                MessageBox.Show("Cliente actualizado exitosamente.",
                    "Actualización exitosa",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
