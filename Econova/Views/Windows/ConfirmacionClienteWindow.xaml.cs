using System.Windows;
using System.Windows.Input;

namespace Econova.Views.Windows
{
    public partial class ConfirmacionClienteWindow : Window
    {
        public bool Confirmado { get; private set; } = false;

        public ConfirmacionClienteWindow(
            string nombre, string cedula,
            string telefono, string email, string direccion)
        {
            InitializeComponent();
            TxtNombre.Text = nombre;
            TxtCedula.Text = cedula;
            TxtTelefono.Text = telefono;
            TxtEmail.Text = email;
            TxtDireccion.Text = direccion;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            Confirmado = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Confirmado = false;
            Close();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}