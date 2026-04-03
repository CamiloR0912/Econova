using System.Windows;
using System.Windows.Input;

namespace Econova
{
    public partial class VentanaEditarCliente : Window
    {
        public Cliente ClienteEditado { get; private set; }

        public VentanaEditarCliente(Cliente cliente)
        {
            InitializeComponent();

            // Cargar datos actuales
            TxtNombres.Text = cliente.Nombres;
            TxtApellidos.Text = cliente.Apellidos;
            TxtCedula.Text = cliente.Cedula;
            TxtTelefono.Text = cliente.Telefono;
            TxtEmail.Text = cliente.Email;
            TxtDireccion.Text = cliente.Direccion;

            ClienteEditado = new Cliente
            {
                Id = cliente.Id,
                Cedula = cliente.Cedula
            };
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNombres.Text) ||
                string.IsNullOrWhiteSpace(TxtApellidos.Text))
            {
                MessageBox.Show("Los nombres y apellidos son obligatorios.",
                    "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ClienteEditado.Nombres = TxtNombres.Text.Trim();
            ClienteEditado.Apellidos = TxtApellidos.Text.Trim();
            ClienteEditado.Telefono = TxtTelefono.Text.Trim();
            ClienteEditado.Email = TxtEmail.Text.Trim();
            ClienteEditado.Direccion = TxtDireccion.Text.Trim();

            this.DialogResult = true;
            this.Close();
        }
    }
}