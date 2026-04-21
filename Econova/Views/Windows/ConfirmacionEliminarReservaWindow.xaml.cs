using System.Windows;
using System.Windows.Input;

namespace Econova.Views.Windows
{
    public partial class ConfirmacionEliminarReservaWindow : Window
    {
        public bool Confirmado { get; private set; } = false;

        public ConfirmacionEliminarReservaWindow(string sala, string cliente, string cedula, string entrada)
        {
            InitializeComponent();
            TxtSala.Text = sala;
            TxtCliente.Text = cliente;
            TxtCedula.Text = cedula;
            TxtEntrada.Text = entrada;
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
