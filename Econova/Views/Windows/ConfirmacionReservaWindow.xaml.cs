// Econova/Views/Dialogs/ConfirmacionReservaWindow.xaml.cs
using System.Windows;

namespace Econova.Views.Dialogs
{
    public partial class ConfirmacionReservaWindow : Window
    {
        public bool Confirmado { get; private set; } = false;

        public ConfirmacionReservaWindow(
            string sala,
            string fechas,
            string cliente)
        {
            InitializeComponent();

            TxtResumenSala.Text = sala;
            TxtResumenFechas.Text = fechas;
            TxtResumenCliente.Text = cliente;
        }

        private void BtnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            Confirmado = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Confirmado = false;
            Close();
        }

        // Permite arrastrar la ventana (sin barra de título nativa)
        protected override void OnMouseLeftButtonDown(
            System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}