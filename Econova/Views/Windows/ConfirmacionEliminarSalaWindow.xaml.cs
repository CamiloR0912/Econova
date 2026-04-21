using System.Windows;
using System.Windows.Input;

namespace Econova.Views.Windows
{
    public partial class ConfirmacionEliminarSalaWindow : Window
    {
        public bool Confirmado { get; private set; } = false;

        public ConfirmacionEliminarSalaWindow(string nombre, int capacidad)
        {
            InitializeComponent();
            TxtNombre.Text = nombre;
            TxtCapacidad.Text = $"{capacidad} personas";
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
