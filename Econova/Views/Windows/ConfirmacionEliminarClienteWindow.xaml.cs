using System.Windows;
using System.Windows.Input;

namespace Econova.Views.Windows
{
    public partial class ConfirmacionEliminarClienteWindow : Window
    {
        public bool Confirmado { get; private set; } = false;

        public ConfirmacionEliminarClienteWindow(string nombre, string cedula, string email)
        {
            InitializeComponent();
            TxtNombre.Text = nombre;
            TxtCedula.Text = cedula;
            TxtEmail.Text = email;
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
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