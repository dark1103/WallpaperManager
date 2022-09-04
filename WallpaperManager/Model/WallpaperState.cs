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
        public string? CurrentImage { get; set; }
        public Dictionary<string, DateTime> UsedImages { get; set; } = new();
        public int CurrentStateIndex { get; 
            set; }


        [JsonIgnore]
        public DateTime? StartTime => CurrentImage != null ? UsedImages[CurrentImage] : null;
    }
}
