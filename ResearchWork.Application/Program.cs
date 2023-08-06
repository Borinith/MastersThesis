using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ResearchWork.IO.Export;
using System;

namespace ResearchWork.Application
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using (var host = Host.CreateDefaultBuilder()
                       .ConfigureServices(services =>
                       {
                           services.AddSingleton<App>();
                           services.AddSingleton<MainWindow>();
                           services.AddTransient<IExportTable, ExportTable>();
                       })
                       .Build())
            {
                var app = host.Services.GetService<App>();

                app?.Run();
            }
        }
    }
}