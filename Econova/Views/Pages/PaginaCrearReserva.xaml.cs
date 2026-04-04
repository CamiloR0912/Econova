using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Econova.ViewModels;

namespace Econova.Views.Pages
{
    public partial class PaginaCrearReserva : Page
    {
        public PaginaCrearReserva()
        {
            InitializeComponent();
            DataContext = new PaginaCrearReservaViewModel();
        }

        private void SoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
    }
}
