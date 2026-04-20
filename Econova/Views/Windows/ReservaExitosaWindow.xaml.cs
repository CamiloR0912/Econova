using System.Windows;
using System.Windows.Input;

namespace Econova.Views.Windows
{
    public partial class ReservaExitosaWindow : Window
    {
        public ReservaExitosaWindow(string sala, string fechas, string cliente)
        {
            InitializeComponent();
            TxtResumenSala.Text = sala;
            TxtResumenFechas.Text = fechas;
            TxtResumenCliente.Text = cliente;
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
            => Close();

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}