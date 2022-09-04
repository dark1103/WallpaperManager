using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WallpaperManager.ViewModel
{
    internal class TrayViewModel
    {
        public ICommand ExitCommand
        {
            get
            {
                return new RelayCommand<CancelEventArgs>((args) => {
                    App.Current.Shutdown();
                });
            }
        }

        public ICommand OpenCommand
        {
            get
            {
                return new RelayCommand<CancelEventArgs>((args) => {
                    var mainWindow = new MainWindow();
                    mainWindow.DataContext = _applicationViewModelAccessor();

                    mainWindow.Show();
                });
            }
        }

        private readonly Func<ApplicationViewModel?> _applicationViewModelAccessor;
        public TrayViewModel(Func<ApplicationViewModel> applicationViewModelProvider)
        {
            _applicationViewModelAccessor = applicationViewModelProvider;
        }
    }
}
