using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Econova.Views.Windows;

namespace Econova.Views.Pages
{
    public partial class PaginaInicio : Page
    {
        public PaginaInicio()
        {
            InitializeComponent();
        }

        private void CardReservas_Click(object sender, MouseButtonEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;
            main?.NavigarAVerReservas();
        }

        private void CardClientes_Click(object sender, MouseButtonEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;
            main?.NavigarAVerClientes();
        }

        private void CardSalas_Click(object sender, MouseButtonEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;
            main?.NavigarASalas();
        }
    }
}