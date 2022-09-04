using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WallpaperManager.Model;

namespace WallpaperManager.Commands
{
    internal class RemovePathCommand : ICommand
    {
        private readonly Action _changeAction;
        public RemovePathCommand(Action changeAction)
        {
            _changeAction = changeAction;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is object[] array && array[0] is Tuple<string, string> path 
                                            && array[1] is WallpaperGroup { Paths: { } } group)
            {
                group.Paths.Remove(path.Item2);

                _changeAction();
            }
        }

        public event EventHandler? CanExecuteChanged;
    }
}
