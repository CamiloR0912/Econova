using System.Windows;
using System.Windows.Controls;
using Econova.ViewModels;
using Econova.Views.Windows;
using Econova.Views.Services;

namespace Econova.Views.Pages
{
    public partial class PaginaVerSalas : Page
    {
        public PaginaVerSalas()
        {
            InitializeComponent();
            DataContext = new PaginaVerSalasViewModel(new WpfDialogService());
        }

        private void BtnAgregarSala_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new VentanaAgregarSala();
            ventana.Owner = Window.GetWindow(this);

            if (ventana.ShowDialog() == true)
            {
                var vm = DataContext as PaginaVerSalasViewModel;
                vm?.AgregarNuevaSala(ventana.NombreSala, ventana.CapacidadSala);

                var confirmacion = new VentanaConfirmacionExito(
                    "La sala ha sido agregada exitosamente.",
                    "Registro exitoso")
                {
                    Owner = Window.GetWindow(this)
                };
                confirmacion.ShowDialog();
            }
        }
    }
}
