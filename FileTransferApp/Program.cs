using FileTransferApp.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using System.Linq;

namespace FileTransferApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            BuildConfig(configBuilder);

            var configurationBuilder = configBuilder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configurationBuilder)
                .Enrich.FromLogContext()
                .WriteTo.File(configurationBuilder.GetValue<string>("Serilog:WriteTo:File"))
                .CreateLogger();

            try
            {
                InitialPresentation();

                Log.Logger.Information("File tranfer starting ...");
                var builder = CreateHostBuilder(args)
                    .Build();

                var _backgroundQueue = builder.Services.GetRequiredService<IBackgroundQueue>();

                builder.StartAsync();

                var monitorTransfer = builder.Services.GetRequiredService<MonitorTransfer>();
                monitorTransfer.StartMonitorTransfer(args);

                builder.WaitForShutdown();

                return;
            }
            catch (Exception ex)
            {
                Log.Fatal("There are problem starting the file transfer ...");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void InitialPresentation()
        {
            Console.WriteLine("***********************    FILE TRANSFER APPLICATION ******************");
            Console.WriteLine(@"**********************  -s [\source dir] -d [\destination dir] *******");
            Console.WriteLine(@"**********************       Press key x to exit    ******************");
            Console.WriteLine("");
            Console.WriteLine("");
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<QueueService>();
                    services.AddScoped<IBackgroundQueue, BackgroundQueue>();
                    services.AddScoped<MonitorTransfer>();
                    services.AddScoped<IHelperService, HelperService>();
                }).UseSerilog();
        }

        private static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: false)
                .AddEnvironmentVariables();
        }
    }
}