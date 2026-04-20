using Econova.Services;
using Econova.Views.Dialogs;
using Econova.Views.Windows;
using System.Windows;

namespace Econova.Views.Services
{
    public class WpfDialogService : IDialogService
    {
        public bool Confirmar(string mensaje, string titulo)
        {
            var resultado = MessageBox.Show(mensaje, titulo,
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return resultado == MessageBoxResult.Yes;
        }

        public void Informar(string mensaje, string titulo)
        {
            MessageBox.Show(mensaje, titulo,
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public bool ConfirmarReserva(string sala, string fechas, string cliente)
        {
            var ventana = new ConfirmacionReservaWindow(sala, fechas, cliente)
            {
                Owner = Application.Current.MainWindow
            };
            ventana.ShowDialog();
            return ventana.Confirmado;
        }

        public void MostrarReservaExitosa(string sala, string fechas, string cliente) // 👈 nuevo
        {
            var ventana = new ReservaExitosaWindow(sala, fechas, cliente)
            {
                Owner = Application.Current.MainWindow
            };
            ventana.ShowDialog();
        }
    }
}