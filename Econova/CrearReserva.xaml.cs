using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Econova
{
    public partial class PaginaCrearReserva : Page
    {
        public PaginaCrearReserva()
        {
            InitializeComponent();
        }

        private void SoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

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

        private void BtnBuscarCliente_Click(object sender, RoutedEventArgs e)
        {
            string cedula = TxtCedula.Text.Trim();

            if (string.IsNullOrEmpty(cedula))
            {
                MessageBox.Show("Por favor ingresa un número de cédula.",
                    "Campo vacío", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ── Aquí conectarás tu base de datos ──
            if (cedula == "1234567890")
            {
                TxtNombreCliente.Text = "Juan Pérez";
                TxtInfoCliente.Text = $"{cedula} • 310 000 0000";
                PanelCliente.Visibility = Visibility.Visible;
            }
            else
            {
                PanelCliente.Visibility = Visibility.Collapsed;
                MessageBox.Show($"No se encontró ningún cliente con la cédula {cedula}.",
                    "Cliente no encontrado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnConfirmarReserva_Click(object sender, RoutedEventArgs e)
        {
            // ── Validaciones ──
            if (CmbSala.SelectedItem == null)
            {
                MessageBox.Show("Por favor selecciona una sala.",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DpFechaEntrada.SelectedDate == null)
            {
                MessageBox.Show("Por favor selecciona la fecha de entrada.",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DpFechaSalida.SelectedDate == null)
            {
                MessageBox.Show("Por favor selecciona la fecha de salida.",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DpFechaSalida.SelectedDate < DpFechaEntrada.SelectedDate)
            {
                MessageBox.Show("La fecha de salida no puede ser anterior a la de entrada.",
                    "Fechas inválidas", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtHoraEntrada.Text, out int hE) || hE < 1 || hE > 12)
            {
                MessageBox.Show("La hora de entrada debe ser entre 01 y 12.",
                    "Hora inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtMinEntrada.Text, out int mE) || mE < 0 || mE > 59)
            {
                MessageBox.Show("Los minutos de entrada deben ser entre 00 y 59.",
                    "Minutos inválidos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtHoraSalida.Text, out int hS) || hS < 1 || hS > 12)
            {
                MessageBox.Show("La hora de salida debe ser entre 01 y 12.",
                    "Hora inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtMinSalida.Text, out int mS) || mS < 0 || mS > 59)
            {
                MessageBox.Show("Los minutos de salida deben ser entre 00 y 59.",
                    "Minutos inválidos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PanelCliente.Visibility != Visibility.Visible)
            {
                MessageBox.Show("Por favor busca y selecciona un cliente válido.",
                    "Cliente requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ── Construir resumen ──
            string horaEntrada = $"{TxtHoraEntrada.Text.PadLeft(2, '0')}:{TxtMinEntrada.Text.PadLeft(2, '0')} {BtnAmPmEntrada.Content}";
            string horaSalida = $"{TxtHoraSalida.Text.PadLeft(2, '0')}:{TxtMinSalida.Text.PadLeft(2, '0')} {BtnAmPmSalida.Content}";
            string sala = (CmbSala.SelectedItem as ComboBoxItem)?.Content?.ToString();

            string resumen =
                $"Por favor verifica los datos antes de confirmar:\n\n" +
                $"  🏠  Sala:       {sala}\n\n" +
                $"  📅  Entrada:   {DpFechaEntrada.SelectedDate:dd/MM/yyyy}  {horaEntrada}\n" +
                $"  📅  Salida:     {DpFechaSalida.SelectedDate:dd/MM/yyyy}  {horaSalida}\n\n" +
                $"  👤  Cliente:   {TxtNombreCliente.Text}\n" +
                $"      Cédula:    {TxtCedula.Text}\n\n" +
                $"¿Deseas confirmar la reserva?";

            var resultado = MessageBox.Show(
                resumen,
                "Confirmar reserva",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                // ── Aquí guardarás la reserva en tu base de datos ──

                MessageBox.Show(
                    "¡Reserva creada exitosamente!",
                    "Reserva confirmada",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                this.NavigationService.Navigate(new PaginaInicio());
            }
        }
    }
}