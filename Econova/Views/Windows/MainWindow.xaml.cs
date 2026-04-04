using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Econova.ViewModels;
using Econova.Views.Pages;

namespace Econova.Views.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
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
            {
                if (btn != null)
                    btn.Style = (Style)FindResource("NavButtonStyle");
            }

            if (botonActivo != null)
                botonActivo.Style = (Style)FindResource("NavButtonActiveStyle");

            //Cierra ambos submenús por defecto
            SubMenuReservas.Visibility = Visibility.Collapsed;
            SubMenuClientes.Visibility = Visibility.Collapsed;
            ArrowReservas.Text = "\uE970";
            ArrowClientes.Text = "\uE970";

            //Reabre solo el submenú que corresponde al botón activo
            if (botonActivo == BtnReservas ||
                botonActivo == BtnCrearReserva ||
                botonActivo == BtnVerReservas)
            {
                SubMenuReservas.Visibility = Visibility.Visible;
                ArrowReservas.Text = "\uE971";
            }
            else if (botonActivo == BtnClientes ||
                     botonActivo == BtnCrearCliente ||
                     botonActivo == BtnVerClientes)
            {
                SubMenuClientes.Visibility = Visibility.Visible;
                ArrowClientes.Text = "\uE971";
            }
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
            // Si ya está abierto, lo cierra (toggle); si no, ActivarBoton lo abre
            bool yaAbierto = SubMenuReservas.Visibility == Visibility.Visible;
            ActivarBoton(BtnReservas);
            if (yaAbierto)
            {
                SubMenuReservas.Visibility = Visibility.Collapsed;
                ArrowReservas.Text = "\uE970";
            }
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
            bool yaAbierto = SubMenuClientes.Visibility == Visibility.Visible;
            ActivarBoton(BtnClientes);
            if (yaAbierto)
            {
                SubMenuClientes.Visibility = Visibility.Collapsed;
                ArrowClientes.Text = "\uE970";
            }
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
            // Navegar a página de salas si existe
        }

        private void BtnManual_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Abriendo manual de usuario...", "Manual", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new VentanaConfirmarSalir();
            ventana.Owner = this;

            if (ventana.ShowDialog() == true)
                Application.Current.Shutdown();
        }
    }
}
