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
    public class StateProvider : IStateProvider
    {
        private const string Filename = "state.json";
        public WallpaperState CurrentState { get; }

        public StateProvider()
        {
            if (File.Exists(Filename))
            {
                CurrentState = JsonConvert.DeserializeObject<WallpaperState>(File.ReadAllText(Filename))!;
            }
            else
            {
                CurrentState = new WallpaperState();
            }

            OnStateChanged += state =>
            {
                var json = JsonConvert.SerializeObject(state);
                File.WriteAllText(Filename, json);
            };
        }

        public event Action<WallpaperState> OnStateChanged = delegate { };
        public void InvokeOnChanged()
        {
            OnStateChanged(CurrentState);
        }
    }
}
