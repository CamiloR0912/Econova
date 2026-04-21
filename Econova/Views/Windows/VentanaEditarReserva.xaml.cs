using System.Windows;
using System.Windows.Input;
using Econova.ViewModels;
using Econova.Models;

namespace Econova.Views.Windows
{
    public partial class VentanaEditarReserva : Window
    {
        public VentanaEditarReserva(Reserva reserva)
        {
            InitializeComponent();
            var vm = new VentanaEditarReservaViewModel(reserva);
            DataContext = vm;
            vm.CloseRequest += (result) =>
            {
                this.DialogResult = result;
                this.Close();
            };
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
