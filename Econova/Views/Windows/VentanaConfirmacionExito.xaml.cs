using System.Windows;
using System.Windows.Input;

namespace Econova.Views.Windows
{
    public partial class VentanaConfirmacionExito : Window
    {
        public VentanaConfirmacionExito(string mensaje, string titulo)
        {
            InitializeComponent();
            TxtTitulo.Text = titulo;
            TxtMensaje.Text = mensaje;
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}
