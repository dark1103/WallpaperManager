using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using KeyboardHookLite;

namespace WallpaperManager.Services
{
    internal class KeyboardHookService
    {
        private readonly IStateProvider _stateProvider;
        private readonly IWallpaperGroupsProvider _wallpaperGroupsProvider;


        public KeyboardHookService(IStateProvider stateProvider, IWallpaperGroupsProvider wallpaperGroupsProvider)
        {
            _stateProvider = stateProvider;
            _wallpaperGroupsProvider = wallpaperGroupsProvider;
        }

        public void Hook()
        {
            var keyboardHook = new KeyboardHook();
            keyboardHook.KeyboardPressed += (sender, args) =>
            {
                if (args.InputEvent.Key == Key.F10 && Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    var maxStateIndex = _wallpaperGroupsProvider.Groups.Max(x => x.StateIndex) + 1;

                    _stateProvider.CurrentState.CurrentStateIndex = (_stateProvider.CurrentState.CurrentStateIndex + 1) % maxStateIndex;
                    _stateProvider.InvokeOnChanged();
                }
            };
        }
    }
}
