using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using WallpaperManager.Commands;
using WallpaperManager.Model;
using WallpaperManager.Services;
using WindowsDisplayAPI;

namespace WallpaperManager.ViewModel
{
    public class ApplicationViewModel : INotifyPropertyChanged, IWallpaperGroupsProvider
    {
        private readonly IWallpaperGroupsProvider _wallpaperGroupsProvider;
        public ICommand RemovePathCommand { get; }
        public ICommand AddFileCommand { get; }
        public ICommand AddFolderCommand { get; }
        public ICommand AddGroupCommand { get; }
        public ICommand RemoveGroupCommand { get; }
        public ICommand WindowClosingCommand
        {
            get
            {
                return new RelayCommand<CancelEventArgs>((args) =>
                {
                    _wallpaperGroupsProvider.Groups = Groups;
                    _wallpaperGroupsProvider.SaveChanges();
                });
            }
        }

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
                    App.Current.MainWindow.Show();
                });
            }
        }

        public ICommand DropCommand
        {
            get
            {
                return new RelayCommand<DragEventArgs>((args) =>
                {
                    var obj = args.Data as DataObject;

                    //get fileName of file saved on disk
                    var fileNames = obj?.GetFileDropList();

                    if (fileNames != null && SelectedGroup != null)
                    {
                        foreach (var fileName in fileNames)
                        {
                            SelectedGroup.Paths ??= new List<string>();

                            if (!SelectedGroup.Paths.Contains(fileName))
                            {
                                SelectedGroup.Paths.Add(fileName);
                            }
                        }
                        OnPropertyChanged(nameof(Groups));
                    }
                });
            }
        }

        public ObservableCollection<WallpaperGroup> Groups { get; }

        IEnumerable<WallpaperGroup> IWallpaperGroupsProvider.Groups
        {
            get => Groups;
            set => throw new NotImplementedException();
        }
        public void AddGroup(WallpaperGroup wallpaperGroup)
        {
            Groups.Add(wallpaperGroup);
        }
        public void RemoveGroup(WallpaperGroup wallpaperGroup)
        {
            Groups.Remove(wallpaperGroup);
        }
        public void SaveChanges()
        {
            throw new NotImplementedException();
        }


        private WallpaperGroup? _selectedGroup;
        public WallpaperGroup? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                _selectedGroup = value;
                OnPropertyChanged();
            }
        }
        
        public IEnumerable<string> AvailableDisplays
        {
            get
            {
                var activeDisplays = Display.GetDisplays().Select(x => x.ToPathDisplayTarget().FriendlyName).ToHashSet();
                activeDisplays.UnionWith(Groups.SelectMany(x => x.Displays ?? Enumerable.Empty<string>()));

                return activeDisplays;
            }
        }

        public ApplicationViewModel() : this(new WallpaperGroupsProvider())
        {

        }

        public ApplicationViewModel(IWallpaperGroupsProvider wallpaperGroupsProvider)
        {
            _wallpaperGroupsProvider = wallpaperGroupsProvider;

            Groups = new ObservableCollection<WallpaperGroup>(_wallpaperGroupsProvider.Groups);

            if (Groups.Any())
                SelectedGroup = Groups.First();

            void GroupsChanged()
            {
                OnPropertyChanged(nameof(Groups));
            }

            RemovePathCommand = new RemovePathCommand(GroupsChanged);

            AddFileCommand = new AddFileCommand(GroupsChanged);
            AddFolderCommand = new AddFolderCommand(GroupsChanged);
            AddGroupCommand = new AddGroupCommand(this, GroupsChanged);
            RemoveGroupCommand = new RemoveGroupCommand(this, GroupsChanged);
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == nameof(Groups))
            {
                if (!Groups.Contains(SelectedGroup))
                {
                    SelectedGroup = Groups.First();
                }
                OnPropertyChanged(nameof(SelectedGroup));
            }
        }
    }
}
