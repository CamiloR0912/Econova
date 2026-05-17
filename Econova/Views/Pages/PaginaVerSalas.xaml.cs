using System.Windows;
using System.Windows.Controls;
using Econova.Models;
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
            var vm = new PaginaVerSalasViewModel(new WpfDialogService());
            vm.OnEditarSala = EditarSala;
            DataContext = vm;
        }

        // RF-06: Abrir ventana de edición de sala
        private bool EditarSala(Sala sala)
        {
            var ventana = new VentanaEditarSala(sala)
            {
                Owner = Window.GetWindow(this)
            };

            if (ventana.ShowDialog() == true)
            {
                var vm = DataContext as PaginaVerSalasViewModel;
                bool actualizada = vm?.ActualizarSala(sala.Id, ventana.NombreSala, ventana.CapacidadSala) == true;
                if (actualizada)
                {
                    var confirmacion = new VentanaConfirmacionExito(
                        "La sala ha sido actualizada exitosamente.",
                        "Actualización exitosa")
                    {
                        Owner = Window.GetWindow(this)
                    };
                    confirmacion.ShowDialog();
                }
                return actualizada;
            }
            return false;
        }

        private void BtnAgregarSala_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new VentanaAgregarSala();
            ventana.Owner = Window.GetWindow(this);

            if (ventana.ShowDialog() == true)
            {
                var vm = DataContext as PaginaVerSalasViewModel;
                bool guardada = vm?.AgregarNuevaSala(ventana.NombreSala, ventana.CapacidadSala) == true;
                if (guardada)
                {
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
}
