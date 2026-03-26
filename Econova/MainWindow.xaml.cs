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

        // Botón X de la title bar — cierra sin confirmar
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Botón "Salir" del sidebar — pide confirmación
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

        // Botón "Manual de usuario" — aquí puedes abrir el PDF o una nueva ventana
        private void BtnManual_Click(object sender, RoutedEventArgs e)
        {
            // Ejemplo: abrir un PDF llamado "manual.pdf" en la carpeta Assets
            // System.Diagnostics.Process.Start("Assets/manual.pdf");

            MessageBox.Show(
                "El manual de usuario estará disponible próximamente.",
                "Manual de usuario",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}