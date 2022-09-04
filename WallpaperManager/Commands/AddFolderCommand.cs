using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Win32;
using WallpaperManager.Model;
using FolderBrowserDialog = FolderBrowserEx.FolderBrowserDialog;

namespace WallpaperManager.Commands
{
    internal class AddFolderCommand : ICommand
    {
        private readonly Action _changeAction;
        public AddFolderCommand(Action changeAction)
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
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
                {
                    AllowMultiSelect = true
                };

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    group.Paths = (group.Paths ?? new List<string>(0))
                        .Concat(folderBrowserDialog.SelectedFolders.Where(Directory.Exists))
                        .Distinct()
                        .ToList();

                    _changeAction();
                }
            }
        }

        public event EventHandler? CanExecuteChanged;
    }
}
