using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ResearchWork.Application.StartCalculation;
using ResearchWork.Calculation.CalculationCO;
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

                           services.AddScoped<IStartCalculation, StartCalculation.StartCalculation>();
                           services.AddScoped<ICalculationX2, CalculationX2>();
                           services.AddScoped<IExport, Export>();
                       })
                       .Build())
            {
                var app = host.Services.GetService<App>();

                app?.Run();
            }
        }
    }
}