using System;
using System.Collections.Generic;
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
            set => _groupsData.Groups = value.ToList();
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

            // _groupsList = new List<WallpaperGroup>()
            // {
            //     new WallpaperGroup()
            //     {
            //         Start = TimeSpan.Parse("11:30"),
            //         End = TimeSpan.Parse("12:40"),
            //         Paths = new List<string>()
            //         {
            //             "C:\\Users\\1\\YandexDisk\\Изображения\\Аниме\\FZ_jwwcacAASWSA.jpg",
            //             "C:\\Users\\1\\YandexDisk\\Изображения\\Аниме\\FKLjprhVQAId9HE.jpg",
            //             "C:\\Users\\1\\YandexDisk\\Изображения\\Аниме\\Фигурки",
            //         }
            //     },
            //     new WallpaperGroup()
            //     {
            //         Start = TimeSpan.Parse("15:30"),
            //         End = TimeSpan.Parse("17:40")
            //     },
            //     new WallpaperGroup()
            //     {
            //         Start = TimeSpan.Parse("19:30"),
            //         End = TimeSpan.Parse("22:40")
            //     }
            // };
        }

        public void AddGroup(WallpaperGroup wallpaperGroup)
        {
            _groupsData.Groups.Add(wallpaperGroup);
        }

        public void RemoveGroup(WallpaperGroup wallpaperGroup)
        {
            _groupsData.Groups.Remove(wallpaperGroup);
        }

        public void SaveChanges()
        {
            var json = JsonConvert.SerializeObject(_groupsData);
            File.WriteAllText(Filename, json);
        }
    }
}
