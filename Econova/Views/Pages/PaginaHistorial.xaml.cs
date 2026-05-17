using System.Windows.Controls;
using Econova.ViewModels;

namespace Econova.Views.Pages
{
    public partial class PaginaHistorial : Page
    {
        public PaginaHistorial()
        {
            InitializeComponent();
            DataContext = new PaginaHistorialViewModel();
        }
    }
}
