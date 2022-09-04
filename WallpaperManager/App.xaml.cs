using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WallpaperManager.Services;
using WallpaperManager.ViewModel;

namespace WallpaperManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;
        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IWallpaperGroupsProvider, WallpaperGroupsProvider>();
                    services.AddSingleton<IStateProvider, StateProvider>();
                    services.AddSingleton<WallpaperUpdateService>();

                    services.AddTransient<ApplicationViewModel>();
                })
                .Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            MainWindow = new MainWindow();
            MainWindow.DataContext = _host.Services.GetService<ApplicationViewModel>();
            MainWindow.Closing += (sender, args) =>
            {
                args.Cancel = true;
                MainWindow.Hide();
            };

            _host.Services.GetService<WallpaperUpdateService>()!.RunTask();

            // MainWindow.Show();


            base.OnStartup(e);
        }
    }
}
