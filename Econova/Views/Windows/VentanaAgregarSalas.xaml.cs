using System.Windows;
using System.Windows.Input;
using Econova.ViewModels;

namespace Econova.Views.Windows
{
    public partial class VentanaAgregarSala : Window
    {
        public string NombreSala { get; private set; }
        public int CapacidadSala { get; private set; }

        public VentanaAgregarSala()
        {
            InitializeComponent();
            var vm = new VentanaAgregarSalaViewModel();
            DataContext = vm;

            vm.SalaGuardada += (nombre, capacidad) =>
            {
                NombreSala = nombre;
                CapacidadSala = capacidad;
                this.DialogResult = true;
                this.Close();
            };

            vm.SolicitudCierre += () =>
            {
                this.DialogResult = false;
                this.Close();
            };
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
