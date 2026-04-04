using System.Windows;
using System.Windows.Input;
using Econova.ViewModels;
using Econova.Models;

namespace Econova.Views.Windows
{
    public partial class VentanaEditarCliente : Window
    {
        public Cliente ClienteEditado { get; private set; }

        public VentanaEditarCliente(Cliente cliente)
        {
            InitializeComponent();
            var vm = new VentanaEditarClienteViewModel(cliente);
            DataContext = vm;
            vm.CloseRequest += (result) =>
            {
                this.DialogResult = result;
                this.Close();
            };
            ClienteEditado = cliente;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
