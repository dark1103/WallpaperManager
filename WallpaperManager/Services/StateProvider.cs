using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WallpaperManager.Model;

namespace WallpaperManager.Services
{
    internal class StateProvider : IStateProvider
    {
        private const string Filename = "state.json";
        public WallpaperState CurrentState { get; }

        public StateProvider()
        {
            if (File.Exists(Filename))
            {
                CurrentState = JsonConvert.DeserializeObject<WallpaperState>(File.ReadAllText(Filename));
            }
            else
            {
                CurrentState = new WallpaperState();
            }

            CurrentState.OnChanged += state =>
            {
                var json = JsonConvert.SerializeObject(state);
                File.WriteAllText(Filename, json);
            };
        }
    }
}
