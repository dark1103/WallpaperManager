using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WallpaperManager.Model;
using WindowsDisplayAPI;

namespace WallpaperManager.Services
{
    internal class WallpaperUpdateService
    {
        private readonly IWallpaperGroupsProvider _wallpaperGroupsProvider;
        private readonly IStateProvider _stateProvider;

        private CancellationTokenSource? _delayCancellationTokenSource;

        public WallpaperUpdateService(IWallpaperGroupsProvider wallpaperGroupsProvider, IStateProvider stateProvider)
        {
            _wallpaperGroupsProvider = wallpaperGroupsProvider;
            _stateProvider = stateProvider;

            _wallpaperGroupsProvider.OnDataChanged += () =>
            {
                _delayCancellationTokenSource?.Cancel();
            };

            _stateProvider.OnStateChanged += state =>
            {
                _delayCancellationTokenSource?.Cancel();
            };
        }

        public void RunTask()
        {
            Task.Run(Run);
        }

        private async void Run()
        {
            while (true)
            {
                string? currentDisplay = Display.GetDisplays().FirstOrDefault(x => x.IsGDIPrimary)?.ToPathDisplayTarget().FriendlyName;

                var newWallpaperGroup = GetNewWallpaperGroup(_wallpaperGroupsProvider.Groups, DateTime.Now, currentDisplay, _stateProvider.CurrentState.CurrentStateIndex);

                double intervalDelay = double.MaxValue;

                if (newWallpaperGroup != null && _stateProvider.CurrentState.Group != newWallpaperGroup)
                {
                    UpdateWallpaper(newWallpaperGroup, out intervalDelay);
                }

                double timeDelay = (TimeSpan.FromDays(1) - DateTime.Now.TimeOfDay).TotalMilliseconds;

                if (_wallpaperGroupsProvider.Groups.Any())
                {
                    timeDelay = _wallpaperGroupsProvider.Groups.Min(x =>
                    {
                        double min = timeDelay;

                        if (x.End > DateTime.Now.TimeOfDay)
                        {
                            min = Math.Min(min, (x.End - DateTime.Now.TimeOfDay).TotalMilliseconds);
                        }

                        if (x.Start > DateTime.Now.TimeOfDay)
                        {
                            min = Math.Min(min, (x.Start - DateTime.Now.TimeOfDay).TotalMilliseconds);
                        }

                        return min;
                    });
                }

                if (_stateProvider.CurrentState.Group != null)
                {
                    var state = _stateProvider.CurrentState;

                    DateTime startTime = state.UsedImages[state.CurrentImage];

                    if (state.Group.Interval > TimeSpan.Zero && (DateTime.Now - startTime) > state.Group.Interval)
                    {
                        UpdateWallpaper(state.Group, out intervalDelay);
                    }
                }

                _delayCancellationTokenSource = new CancellationTokenSource();

                await Task.Delay((int)Math.Min(intervalDelay, timeDelay), _delayCancellationTokenSource.Token).ContinueWith(tsk => { });
            }
        }

        private void UpdateWallpaper(WallpaperGroup wallpaperGroup, out double updateDelay)
        {
            var state = _stateProvider.CurrentState;
            state.Group = wallpaperGroup;

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
                var unfinishedImage = state.UsedImages.FirstOrDefault(x => DateTime.Now - x.Value < wallpaperGroup.Interval && allImages.Contains(x.Key)).Key;

                // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
                newImage = unfinishedImage ?? images[Random.Shared.Next(images.Count)];
            }
            else
            {
                newImage = images[0];
            }

            WallpaperWindowsApi.Set(newImage, WallpaperWindowsApi.Style.Span);

            state.CurrentImage = newImage;

            if (!state.UsedImages.ContainsKey(newImage))
            {
                state.UsedImages.Add(newImage, DateTime.Now);
            }

            updateDelay = wallpaperGroup.Interval != TimeSpan.Zero ? wallpaperGroup.Interval.TotalMilliseconds : double.MaxValue;

            _stateProvider.InvokeOnChanged();
        }

        private WallpaperGroup? GetNewWallpaperGroup(IEnumerable<WallpaperGroup> wallpaperGroups, DateTime now, string? currentDisplay, int currentStateIndex)
        {
            return wallpaperGroups.FirstOrDefault(x=> 
                x.StateIndex == currentStateIndex 
                && x.AllImages.Any()
                && (x.Start == x.End 
                    || (x.Start < x.End 
                        ? (now.TimeOfDay >= x.Start && now.TimeOfDay < x.End) 
                        : (now.TimeOfDay <= x.Start || now.TimeOfDay > x.End)))
                && (x.Displays?.Any() != true || x.Displays?.Contains(currentDisplay!) != false));
        }
    }
}
