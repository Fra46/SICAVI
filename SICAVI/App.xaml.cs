using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SICAVI.WinUI;
using SICAVI.DAL.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;

namespace SICAVI
{
    public partial class App : Application
    {
        public static Window? MainWindow { get; private set; }

        public static IServiceProvider Services { get; private set; }

        public App()
        {
            this.InitializeComponent();

            var services = new ServiceCollection();

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SICAVI"
            );

            Directory.CreateDirectory(folder);
            var dbPath = Path.Combine(
                Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                "sicavi.db"
            );

            services.AddDbContext<ConnectionContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            Services = services.BuildServiceProvider();

            using (var scope = Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ConnectionContext>();
                db.Database.Migrate();
            }
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            MainWindow = new MainWindow();
            MainWindow.Activate();
        }
    }
}
