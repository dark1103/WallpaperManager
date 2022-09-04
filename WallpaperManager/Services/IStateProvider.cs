using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Model;

namespace WallpaperManager.Services
{
    internal interface IStateProvider
    {
        WallpaperState CurrentState { get; }
    }
}
