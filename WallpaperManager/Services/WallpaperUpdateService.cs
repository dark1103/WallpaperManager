using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Model;
using WindowsDisplayAPI;

namespace WallpaperManager.Services
{
    internal class WallpaperUpdateService
    {
        private readonly IWallpaperGroupsProvider _wallpaperGroupsProvider;
        private readonly IStateProvider _stateProvider;

        public WallpaperUpdateService(IWallpaperGroupsProvider wallpaperGroupsProvider, IStateProvider stateProvider)
        {
            _wallpaperGroupsProvider = wallpaperGroupsProvider;
            _stateProvider = stateProvider;
        }

        public void RunTask()
        {
            Task.Run(Run);
        }

        private async void Run()
        {
            while (true)
            {
                var currentDisplay = Display.GetDisplays().FirstOrDefault(x => x.IsGDIPrimary)?.ToPathDisplayTarget().FriendlyName;

                var newWallpaperGroup = GetNewWallpaperGroup(_wallpaperGroupsProvider.Groups, DateTime.Now, currentDisplay);

                if (newWallpaperGroup != null && _stateProvider.CurrentState.Group != newWallpaperGroup)
                {
                    UpdateWallpaper(newWallpaperGroup);
                }

                if (_stateProvider.CurrentState.Group != null)
                {
                    var state = _stateProvider.CurrentState;

                    if (state.Group.Interval > TimeSpan.Zero && (DateTime.Now - state.StartTime) > state.Group.Interval)
                    {
                        UpdateWallpaper(state.Group);
                    }
                }

                await Task.Delay(5000);
            }
        }

        private void UpdateWallpaper(WallpaperGroup wallpaperGroup)
        {
            var state = _stateProvider.CurrentState;
            state.Group = wallpaperGroup;
            state.StartTime = DateTime.Now;

            var allImages = wallpaperGroup.AllImages.Select(x => x.Item1).ToList();


            var images = allImages.Except(state.UsedImages.Where(x=> DateTime.Now - x.Value >= wallpaperGroup.Interval).Select(x=>x.Key)).ToList();

            if (!images.Any())
            {
                images = allImages;

                foreach (var image in images)
                {
                    state.UsedImages.Remove(image);
                }
            }

            string newImage;

            if (wallpaperGroup.IsRandom)
            {
                var unfinishedImage = state.UsedImages.FirstOrDefault(x => DateTime.Now - x.Value < wallpaperGroup.Interval).Key;

                newImage = unfinishedImage ?? images[Random.Shared.Next(images.Count)];
            }
            else
            {
                newImage = images[0];
            }

            WallpaperWindowsApi.Set(newImage, WallpaperWindowsApi.Style.Span);

            if (!state.UsedImages.ContainsKey(newImage))
            {
                state.UsedImages.Add(newImage, DateTime.Now);
            }

            state.InvokeOnChanged();
        }

        private WallpaperGroup? GetNewWallpaperGroup(IEnumerable<WallpaperGroup> wallpaperGroups, DateTime now, string currentDisplay)
        {
            return wallpaperGroups.FirstOrDefault(x=> (x.Start == x.End || x.Start < x.End ? (now.TimeOfDay >= x.Start && now.TimeOfDay < x.End) : (now.TimeOfDay <= x.Start && now.TimeOfDay > x.End)) 
                                                      && x.Displays?.Any() != true || x.Displays?.Contains(currentDisplay) != false);
        }
    }
}
