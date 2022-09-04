using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using KeyboardHookLite;
using static System.Windows.Forms.AxHost;

namespace WallpaperManager.Services
{
    internal class KeyboardHookService
    {
        private readonly IStateProvider _stateProvider;
        private readonly IWallpaperGroupsProvider _wallpaperGroupsProvider;

        private KeyboardHook? _keyboardHook;
        public KeyboardHookService(IStateProvider stateProvider, IWallpaperGroupsProvider wallpaperGroupsProvider)
        {
            _stateProvider = stateProvider;
            _wallpaperGroupsProvider = wallpaperGroupsProvider;
        }

        public void Hook()
        {
            _keyboardHook = new KeyboardHook();
            _keyboardHook.KeyboardPressed += (sender, args) =>
            {
                if (args.KeyPressType == KeyboardHook.KeyPressType.KeyDown && args.InputEvent.Key == Key.F10 && Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    var maxStateIndex = _wallpaperGroupsProvider.Groups.Max(x => x.StateIndex) + 1;

                    if (maxStateIndex < 2 && _wallpaperGroupsProvider.Groups.Any(x=>x.StateIndex == -1))
                    {
                        maxStateIndex++;
                    }

                    _stateProvider.CurrentState.CurrentStateIndex = (_stateProvider.CurrentState.CurrentStateIndex + 1) % maxStateIndex;

                    _stateProvider.InvokeOnChanged();
                }
            };
        }
    }
}
