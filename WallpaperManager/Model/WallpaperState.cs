using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WallpaperManager.Model
{
    public class WallpaperState
    {
        [JsonIgnore]
        public WallpaperGroup? Group { get; set; }
        public DateTime StartTime { get; set; }
        public Dictionary<string, DateTime> UsedImages { get; set; } = new();


        public event Action<WallpaperState> OnChanged = delegate {  };
        public void InvokeOnChanged()
        {
            OnChanged(this);
        }
    }
}
