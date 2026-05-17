using System.Windows;
using System.Windows.Input;
using Econova.Models;

namespace Econova.Views.Windows
{
    public partial class VentanaEditarSala : Window
    {
        public string NombreSala { get; private set; }
        public int CapacidadSala { get; private set; }
        public bool Guardado { get; private set; }

        public VentanaEditarSala(Sala sala)
        {
            InitializeComponent();
            TxtNombre.Text = sala.Nombre;
            TxtCapacidad.Text = sala.Capacidad.ToString();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = TxtNombre.Text?.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                TxtError.Text = "El nombre de la sala es obligatorio.";
                return;
            }

            if (!int.TryParse(TxtCapacidad.Text?.Trim(), out int capacidad) || capacidad <= 0)
            {
                TxtError.Text = "La capacidad debe ser un número mayor a 0.";
                return;
            }

            NombreSala = nombre;
            CapacidadSala = capacidad;
            Guardado = true;
            DialogResult = true;
            Close();
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
