using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using WallpaperManager.Model;

namespace WallpaperManager.Commands
{
    internal class AddFileCommand : ICommand
    {
        private readonly Action _changeAction;
        public AddFileCommand(Action changeAction)
        {
            _changeAction = changeAction;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is WallpaperGroup group)
            {
                var openFileDialog = new OpenFileDialog()
                {
                    Multiselect = true
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    group.Paths = (group.Paths ?? new List<string>(0))
                        .Concat(openFileDialog.FileNames.Where(File.Exists))
                        .Distinct()
                        .ToList();
                    
                    _changeAction();
                }
            }
        }

        public event EventHandler? CanExecuteChanged;
    }
}
