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
                    services.AddTransient(x=>new TrayViewModel(x.GetRequiredService<ApplicationViewModel>));
                })
                .Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            // MainWindow = new MainWindow();
            // MainWindow.DataContext = _host.Services.GetService<ApplicationViewModel>();

            _host.Services.GetService<WallpaperUpdateService>()!.RunTask();

            MainWindow = new TrayWindow()
            {
                DataContext = _host.Services.GetService<TrayViewModel>()
            };


            base.OnStartup(e);
        }
    }
}
