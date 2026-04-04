using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Econova.ViewModels;

namespace Econova.Views.Pages
{
    public partial class PaginaCrearCliente : Page
    {
        public PaginaCrearCliente()
        {
            InitializeComponent();
            DataContext = new PaginaCrearClienteViewModel();
        }

        private void SoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
    }
}