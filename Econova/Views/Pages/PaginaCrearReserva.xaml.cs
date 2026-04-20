using Econova.ViewModels;
using Econova.Views.Services;
using Econova.Views.Windows;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Econova.Views.Pages
{
    public partial class PaginaCrearReserva : Page
    {
        public PaginaCrearReserva()
        {
            InitializeComponent();
            var vm = new PaginaCrearReservaViewModel(new WpfDialogService());

            // 👇 Conecta la navegación: busca el MainWindow y navega al inicio
            vm.NavegaAlInicio = () =>
            {
                if (Application.Current.MainWindow is MainWindow main)
                {
                    main.NavigarAlInicio(); // método que añadiremos en el paso 7
                }
            };

            DataContext = vm;
        }

        // 👇 Alterna el botón AM/PM de entrada
        private void BtnAmPmEntrada_Click(object sender, RoutedEventArgs e)
        {
            if (BtnAmPmEntrada.Content.ToString() == "AM")
            {
                BtnAmPmEntrada.Content = "PM";
                BtnAmPmEntrada.Style = (Style)FindResource("AmPmButtonStyle");
            }
            else
            {
                BtnAmPmEntrada.Content = "AM";
                BtnAmPmEntrada.Style = (Style)FindResource("AmPmButtonActiveStyle");
            }
        }

        // 👇 Alterna el botón AM/PM de salida
        private void BtnAmPmSalida_Click(object sender, RoutedEventArgs e)
        {
            if (BtnAmPmSalida.Content.ToString() == "AM")
            {
                BtnAmPmSalida.Content = "PM";
                BtnAmPmSalida.Style = (Style)FindResource("AmPmButtonStyle");
            }
            else
            {
                BtnAmPmSalida.Content = "AM";
                BtnAmPmSalida.Style = (Style)FindResource("AmPmButtonActiveStyle");
            }
        }

        // 👇 Permite solo dígitos en los campos de hora y minutos
        private void SoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
        }
    }
}