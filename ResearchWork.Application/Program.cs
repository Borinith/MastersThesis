using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                       })
                       .Build())
            {
                var app = host.Services.GetService<App>();

                app?.Run();
            }
        }
    }
}