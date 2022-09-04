using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WallpaperManager.Model;

namespace WallpaperManager.Services
{
    internal class WallpaperGroupsProvider : IWallpaperGroupsProvider
    {
        private const string Filename = "config.json";

        private WallpaperGroupsData _groupsData;

        public IEnumerable<WallpaperGroup> Groups
        {
            get => _groupsData.Groups;
            set
            {
                _groupsData.Groups = value.ToList();
                OnDataChanged();
            }
        }


        public WallpaperGroupsProvider()
        {
            if (File.Exists(Filename))
            {
                _groupsData = JsonConvert.DeserializeObject<WallpaperGroupsData>(File.ReadAllText(Filename));
            }
            else
            {
                _groupsData = new WallpaperGroupsData();
            }
        }

        public void AddGroup(WallpaperGroup wallpaperGroup)
        {
            _groupsData.Groups.Add(wallpaperGroup);
            OnDataChanged();
        }

        public void RemoveGroup(WallpaperGroup wallpaperGroup)
        {
            _groupsData.Groups.Remove(wallpaperGroup);
            OnDataChanged();
        }

        public void SaveChanges()
        {
            var json = JsonConvert.SerializeObject(_groupsData);
            File.WriteAllText(Filename, json);
        }

        public event Action OnDataChanged = delegate {  };
    }
}
