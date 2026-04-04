using Econova.Core;

namespace Econova.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public MainWindowViewModel()
        {
            // Initial view could be set here, but we'll let MainWindow handle it for now
            // to maintain the Frame navigation if needed.
        }
    }
}
