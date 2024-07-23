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
            // Создаем билдер
            var builder = Host.CreateApplicationBuilder();

            // Внедряем сервисы
            builder.Services.AddSingleton<App>();
            builder.Services.AddSingleton<MainWindow>();

            builder.Services.AddScoped<IStartCalculation, StartCalculation.StartCalculation>();
            builder.Services.AddScoped<ICalculationX2, CalculationX2>();
            builder.Services.AddScoped<IExport, Export>();

            // Создаем хост приложения
            using var host = builder.Build();

            // Получаем сервис - объект класса App
            var app = host.Services.GetService<App>();

            // Запускаем приложение
            app?.Run();
        }
    }
}