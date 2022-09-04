using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Model;

namespace WallpaperManager.Services
{
    public interface IStateProvider
    {
        WallpaperState CurrentState { get; }
        event Action<WallpaperState> OnStateChanged;
        void InvokeOnChanged();
    }
}
