using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Econova
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Cargar inicio y marcarlo como activo
            ActivarBoton(BtnInicio);
            FrameContenido.Navigate(new PaginaInicio());
        }

        // ── Activa visualmente un botón y desactiva los demás ──
        private void ActivarBoton(Button botonActivo)
        {
            Button[] botones = {
                BtnInicio, BtnReservas, BtnCrearReserva, BtnVerReservas,
                BtnClientes, BtnCrearCliente, BtnVerClientes, BtnSalas
            };

            foreach (var btn in botones)
                btn.Style = (Style)FindResource("NavButtonStyle");

            botonActivo.Style = (Style)FindResource("NavButtonActiveStyle");
        }

        // ── Title bar ──
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                BtnMaximize_Click(sender, e);
            else
                this.DragMove();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
            => this.WindowState = WindowState.Minimized;

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
            => this.Close();

        // ── Navegación ──
        private void BtnInicio_Click(object sender, RoutedEventArgs e)
        {
            ActivarBoton(BtnInicio);
            FrameContenido.Navigate(new PaginaInicio());
        }

        private void BtnReservas_Click(object sender, RoutedEventArgs e)
        {
            bool abierto = SubMenuReservas.Visibility == Visibility.Visible;
            SubMenuReservas.Visibility = abierto ? Visibility.Collapsed : Visibility.Visible;
            ArrowReservas.Text = abierto ? "\uE970" : "\uE971";
            ActivarBoton(BtnReservas);
        }

        private void BtnCrearReserva_Click(object sender, RoutedEventArgs e)
        {
            ActivarBoton(BtnCrearReserva);
            FrameContenido.Navigate(new PaginaCrearReserva());
        }

        private void BtnVerReservas_Click(object sender, RoutedEventArgs e)
        {
            ActivarBoton(BtnVerReservas);
            FrameContenido.Navigate(new PaginaVerReservas());
        }

        private void BtnClientes_Click(object sender, RoutedEventArgs e)
        {
            bool abierto = SubMenuClientes.Visibility == Visibility.Visible;
            SubMenuClientes.Visibility = abierto ? Visibility.Collapsed : Visibility.Visible;
            ArrowClientes.Text = abierto ? "\uE970" : "\uE971";
            ActivarBoton(BtnClientes);
        }

        private void BtnCrearCliente_Click(object sender, RoutedEventArgs e)
        {
            ActivarBoton(BtnCrearCliente);
            FrameContenido.Navigate(new PaginaCrearCliente());
        }

        private void BtnVerClientes_Click(object sender, RoutedEventArgs e)
        {
            ActivarBoton(BtnVerClientes);
            FrameContenido.Navigate(new PaginaVerClientes());
        }

        private void BtnSalas_Click(object sender, RoutedEventArgs e)
        {
            ActivarBoton(BtnSalas);
            // FrameContenido.Navigate(new PaginaSalas());
        }

        // ── Salir y Manual ──
        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show(
                "¿Estás seguro de que deseas cerrar la aplicación?",
                "Confirmar cierre",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
                this.Close();
        }

        private void BtnManual_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "El manual de usuario estará disponible próximamente.",
                "Manual de usuario",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}