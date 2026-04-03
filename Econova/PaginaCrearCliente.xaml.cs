using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Econova
{
    public partial class PaginaCrearCliente : Page
    {
        public PaginaCrearCliente()
        {
            InitializeComponent();
        }

        private void SoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            TxtNombres.Text = string.Empty;
            TxtApellidos.Text = string.Empty;
            TxtCedula.Text = string.Empty;
            TxtTelefono.Text = string.Empty;
            TxtEmail.Text = string.Empty;
            TxtDireccion.Text = string.Empty;
            TxtNombres.Focus();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // ── Validaciones ──
            if (string.IsNullOrWhiteSpace(TxtNombres.Text))
            {
                MessageBox.Show("Por favor ingresa los nombres del cliente.",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtNombres.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtApellidos.Text))
            {
                MessageBox.Show("Por favor ingresa los apellidos del cliente.",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtApellidos.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtCedula.Text) || TxtCedula.Text.Length < 6)
            {
                MessageBox.Show("Por favor ingresa una cédula válida (mínimo 6 dígitos).",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtCedula.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtTelefono.Text) || TxtTelefono.Text.Length < 7)
            {
                MessageBox.Show("Por favor ingresa un teléfono válido (mínimo 7 dígitos).",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtTelefono.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtEmail.Text) ||
                !Regex.IsMatch(TxtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Por favor ingresa un correo electrónico válido.",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtDireccion.Text))
            {
                MessageBox.Show("Por favor ingresa la dirección del cliente.",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtDireccion.Focus();
                return;
            }

            // ── Ventana de confirmación ──
            string resumen =
                $"Por favor verifica los datos antes de guardar:\n\n" +
                $"  Nombres:    {TxtNombres.Text} {TxtApellidos.Text}\n" +
                $"  Cédula:     {TxtCedula.Text}\n" +
                $"  Teléfono:   {TxtTelefono.Text}\n" +
                $"  Email:      {TxtEmail.Text}\n" +
                $"  Dirección:  {TxtDireccion.Text}\n\n" +
                $"¿Deseas guardar este cliente?";

            var resultado = MessageBox.Show(
                resumen,
                "Confirmar cliente",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                // ── Aquí guardarás el cliente en tu base de datos ──

                MessageBox.Show(
                    $"¡Cliente {TxtNombres.Text} {TxtApellidos.Text} registrado exitosamente!",
                    "Cliente guardado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                this.NavigationService.Navigate(new PaginaInicio());
            }
        }
    }
}