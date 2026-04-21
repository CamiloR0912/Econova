using Econova.Models;
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

        public void MostrarReservaExitosa(string sala, string fechas, string cliente)
        {
            var ventana = new ReservaExitosaWindow(sala, fechas, cliente)
            {
                Owner = Application.Current.MainWindow
            };
            ventana.ShowDialog();
        }

        public bool ConfirmarEliminarReserva(string sala, string cliente, string cedula, string entrada)
        {
            var ventana = new ConfirmacionEliminarReservaWindow(sala, cliente, cedula, entrada)
            {
                Owner = Application.Current.MainWindow
            };
            ventana.ShowDialog();
            return ventana.Confirmado;
        }

        public bool EditarReserva(Reserva reserva)
        {
            var ventana = new VentanaEditarReserva(reserva)
            {
                Owner = Application.Current.MainWindow
            };
            return ventana.ShowDialog() == true;
        }

        public bool ConfirmarEliminarSala(string nombre, int capacidad)
        {
            var ventana = new ConfirmacionEliminarSalaWindow(nombre, capacidad)
            {
                Owner = Application.Current.MainWindow
            };
            ventana.ShowDialog();
            return ventana.Confirmado;
        }

        public void MostrarConfirmacionExito(string mensaje, string titulo)
        {
            var ventana = new VentanaConfirmacionExito(mensaje, titulo)
            {
                Owner = Application.Current.MainWindow
            };
            ventana.ShowDialog();
        }
    }
}