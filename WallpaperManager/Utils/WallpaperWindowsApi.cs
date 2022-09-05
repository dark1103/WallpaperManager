using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace WallpaperManager
{
    public static class WallpaperWindowsApi
    {

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched, 
            Span
        }

        public static void Set(string path, Style style)
        {

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);


            int st = style switch
            {
                Style.Tiled => 1,
                Style.Centered => 1,
                Style.Stretched => 2,
                Style.Span => 22
            };

            int tile = style switch
            {
                Style.Tiled => 1,
                _ => 0
            };

            key.SetValue(@"WallpaperStyle", st.ToString());
            key.SetValue(@"TileWallpaper", tile.ToString());

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                path,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }

}
