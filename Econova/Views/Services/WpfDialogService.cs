using System.Windows;
using Econova.Services;

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
    }
}