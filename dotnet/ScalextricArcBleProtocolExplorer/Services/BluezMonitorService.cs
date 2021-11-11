using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.Services
{
    public class BluezMonitorService : BackgroundService
    {
        private readonly ILogger<BluezMonitorService> _logger;


        public BluezMonitorService(ILogger<BluezMonitorService> logger)
        {
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var services = await Tmds.DBus.Connection.System.ListServicesAsync();
                    Console.WriteLine($"Number of services: {services.Length}");
                    foreach (var service in services)
                    {
                        Console.WriteLine(service);
                    }

                    var activatableServices = await Tmds.DBus.Connection.System.ListActivatableServicesAsync();
                    Console.WriteLine($"Number of activatable services: {activatableServices.Length}");
                    foreach (var activatableService in activatableServices)
                    {
                        Console.WriteLine(activatableServices);
                    }

                    _logger.LogInformation("BluezMonitorService is running...");
                    await Task.Delay(10000, cancellationToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.Message);
                    await Task.Delay(10000, cancellationToken);
                }
            }
        }
    }
}
