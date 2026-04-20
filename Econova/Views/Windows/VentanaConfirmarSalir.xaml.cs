using System.Windows;

namespace Econova.Views.Windows
{
    public partial class VentanaConfirmarSalir : Window
    {
        public VentanaConfirmarSalir()
        {
            InitializeComponent();
        }

        private void BtnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}