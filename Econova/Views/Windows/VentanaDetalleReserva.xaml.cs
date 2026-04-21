using System.Windows;
using System.Windows.Input;
using Econova.Models;

namespace Econova.Views.Windows
{
    public partial class VentanaDetalleReserva : Window
    {
        public VentanaDetalleReserva(Reserva reserva)
        {
            InitializeComponent();
            TxtTitulo.Text = $"Detalle de reserva #{reserva.Numero}";
            TxtSala.Text = reserva.Sala;
            TxtCliente.Text = reserva.Cliente;
            TxtCedula.Text = reserva.Cedula;
            TxtEntrada.Text = $"{reserva.FechaEntrada}  {reserva.HoraEntrada}";
            TxtSalida.Text = $"{reserva.FechaSalida}  {reserva.HoraSalida}";
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
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
