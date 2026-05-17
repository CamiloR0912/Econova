using System.Windows.Controls;
using System.Windows.Input;
using Econova.ViewModels;

namespace Econova.Views.Pages
{
    public partial class PaginaPanelSalas : Page
    {
        public PaginaPanelSalas()
        {
            InitializeComponent();
            DataContext = new PaginaPanelSalasViewModel();
        }

        private void SalaCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.FrameworkElement element && element.Tag is int id)
            {
                var vm = DataContext as PaginaPanelSalasViewModel;
                vm?.SeleccionarSalaCommand.Execute(id);
            }
        }
    }
}
