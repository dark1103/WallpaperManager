using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Model;

namespace WallpaperManager.Services
{
    public interface IWallpaperGroupsProvider
    {
        IEnumerable<WallpaperGroup> Groups { get; set; }

        void AddGroup(WallpaperGroup wallpaperGroup);
        void RemoveGroup(WallpaperGroup wallpaperGroup);
        void SaveChanges();
    }
}
