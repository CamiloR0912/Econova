using Econova.ViewModels;
using Econova.Views.Services;
using Econova.Views.Windows;
using System;
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

        private void BtnConfirmarReserva_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is PaginaCrearReservaViewModel vm))
                return;

            if (!DpFechaEntrada.SelectedDate.HasValue || !DpFechaSalida.SelectedDate.HasValue)
            {
                MessageBox.Show("Debes seleccionar fecha de entrada y salida.",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtHoraEntrada.Text, out int horaEntrada) ||
                !int.TryParse(TxtMinEntrada.Text, out int minEntrada) ||
                !int.TryParse(TxtHoraSalida.Text, out int horaSalida) ||
                !int.TryParse(TxtMinSalida.Text, out int minSalida))
            {
                MessageBox.Show("Hora o minutos inválidos.",
                    "Formato inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (horaEntrada < 1 || horaEntrada > 12 || horaSalida < 1 || horaSalida > 12 ||
                minEntrada < 0 || minEntrada > 59 || minSalida < 0 || minSalida > 59)
            {
                MessageBox.Show("Revisa los valores de hora (1-12) y minutos (00-59).",
                    "Formato inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool entradaPm = string.Equals(BtnAmPmEntrada.Content?.ToString(), "PM", StringComparison.OrdinalIgnoreCase);
            bool salidaPm = string.Equals(BtnAmPmSalida.Content?.ToString(), "PM", StringComparison.OrdinalIgnoreCase);

            int horaEntrada24 = ConvertirHora24(horaEntrada, entradaPm);
            int horaSalida24 = ConvertirHora24(horaSalida, salidaPm);

            var fechaEntrada = DpFechaEntrada.SelectedDate.Value.Date
                .AddHours(horaEntrada24)
                .AddMinutes(minEntrada);
            var fechaSalida = DpFechaSalida.SelectedDate.Value.Date
                .AddHours(horaSalida24)
                .AddMinutes(minSalida);

            vm.ConfirmarReserva(fechaEntrada, fechaSalida);
        }

        private static int ConvertirHora24(int hora12, bool esPm)
        {
            if (hora12 == 12)
                return esPm ? 12 : 0;

            return esPm ? hora12 + 12 : hora12;
        }
    }
}