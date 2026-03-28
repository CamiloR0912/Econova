using System.Windows;
using System.Windows.Input;

namespace Econova
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                BtnMaximize_Click(sender, e);
            else
                this.DragMove();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnReservas_Click(object sender, RoutedEventArgs e)
        {
            bool abierto = SubMenuReservas.Visibility == Visibility.Visible;
            SubMenuReservas.Visibility = abierto ? Visibility.Collapsed : Visibility.Visible;
            ArrowReservas.Text = abierto ? "\uE970" : "\uE971";
        }

        private void BtnClientes_Click(object sender, RoutedEventArgs e)
        {
            bool abierto = SubMenuClientes.Visibility == Visibility.Visible;
            SubMenuClientes.Visibility = abierto ? Visibility.Collapsed : Visibility.Visible;
            ArrowClientes.Text = abierto ? "\uE970" : "\uE971";
        }

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
            // Para abrir un PDF: System.Diagnostics.Process.Start("Assets/manual.pdf");
            MessageBox.Show(
                "El manual de usuario estará disponible próximamente.",
                "Manual de usuario",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}