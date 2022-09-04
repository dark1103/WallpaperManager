using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WallpaperManager.Model;
using WallpaperManager.Services;

namespace WallpaperManager.Commands
{
    internal class RemoveGroupCommand : ICommand
    {
        private readonly IWallpaperGroupsProvider _provider;
        private readonly Action _changeAction;

        public RemoveGroupCommand(IWallpaperGroupsProvider provider, Action changeAction)
        {
            _provider = provider;
            _changeAction = changeAction;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _provider.RemoveGroup((WallpaperGroup)parameter);
            _changeAction();
        }

        public event EventHandler? CanExecuteChanged;
    }
}
