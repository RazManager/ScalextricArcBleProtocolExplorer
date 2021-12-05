using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Channels;


var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging(loggingBuilder =>
{
    loggingBuilder.AddProvider(new ScalextricArcBleProtocolExplorer.Services.MemoryLogger.MemoryLoggerProvider());
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

builder.Services.AddSingleton(serviceProvider =>
    new Queue<ScalextricArcBleProtocolExplorer.Services.MemoryLogger.MemoryLoggerData>()
);

builder.Services.AddSingleton(serviceProvider =>
    Channel.CreateBounded<ScalextricArcBleProtocolExplorer.Services.CarIdState>(new BoundedChannelOptions(10)
    {
        FullMode = BoundedChannelFullMode.DropOldest,
        SingleWriter = false,
        SingleReader = true
    })
);

builder.Services.AddSingleton(serviceProvider =>
    Channel.CreateBounded<ScalextricArcBleProtocolExplorer.Services.CommandState>(new BoundedChannelOptions(10)
    {
        FullMode = BoundedChannelFullMode.DropOldest,
        SingleWriter = false,
        SingleReader = true
    })
);

builder.Services.AddSingleton(serviceProvider =>
    Channel.CreateBounded<ScalextricArcBleProtocolExplorer.Services.ThrottleProfileState>(new BoundedChannelOptions(10)
    {
        FullMode = BoundedChannelFullMode.DropOldest,
        SingleWriter = false,
        SingleReader = true
    })
);

builder.Services.AddSingleton(serviceProvider =>
    new ScalextricArcBleProtocolExplorer.Services.ScalextricArcState
    (
        serviceProvider.GetRequiredService<IHubContext<ScalextricArcBleProtocolExplorer.Hubs.CarIdHub, ScalextricArcBleProtocolExplorer.Hubs.ICarIdHub>>(),
        serviceProvider.GetRequiredService<Channel<ScalextricArcBleProtocolExplorer.Services.CarIdState>>(),
        serviceProvider.GetRequiredService<IHubContext<ScalextricArcBleProtocolExplorer.Hubs.CommandHub, ScalextricArcBleProtocolExplorer.Hubs.ICommandHub>>(),
        serviceProvider.GetRequiredService<Channel<ScalextricArcBleProtocolExplorer.Services.CommandState>>(),
        serviceProvider.GetRequiredService<IHubContext<ScalextricArcBleProtocolExplorer.Hubs.ConnectionHub, ScalextricArcBleProtocolExplorer.Hubs.IConnectionHub>>(),
        serviceProvider.GetRequiredService<IHubContext<ScalextricArcBleProtocolExplorer.Hubs.SlotHub, ScalextricArcBleProtocolExplorer.Hubs.ISlotHub>>(),
        serviceProvider.GetRequiredService<IHubContext<ScalextricArcBleProtocolExplorer.Hubs.ThrottleHub, ScalextricArcBleProtocolExplorer.Hubs.IThrottleHub>>(),
        serviceProvider.GetRequiredService<IHubContext<ScalextricArcBleProtocolExplorer.Hubs.ThrottleProfileHub, ScalextricArcBleProtocolExplorer.Hubs.IThrottleProfileHub>>(),
        serviceProvider.GetRequiredService<Channel<ScalextricArcBleProtocolExplorer.Services.ThrottleProfileState>>(),
        serviceProvider.GetRequiredService<IHubContext<ScalextricArcBleProtocolExplorer.Hubs.TrackHub, ScalextricArcBleProtocolExplorer.Hubs.ITrackHub>>()
    )
);

builder.Services.AddSingleton<ScalextricArcBleProtocolExplorer.Services.CpuInfo.ICpuInfoService>(serviceProvider =>
    new ScalextricArcBleProtocolExplorer.Services.CpuInfo.CpuInfoService(serviceProvider.GetRequiredService<ILogger<ScalextricArcBleProtocolExplorer.Services.CpuInfo.CpuInfoService>>())
);
builder.Services.AddHostedService<ScalextricArcBleProtocolExplorer.Services.MemoryLogger.MemoryLoggerService>();
builder.Services.AddHostedService<ScalextricArcBleProtocolExplorer.Services.BluezMonitorService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

builder.Services.AddSignalR()
    .AddJsonProtocol(options => {
        options.PayloadSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors();
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors(builder =>
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials());
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == StatusCodes.Status404NotFound &&
        context.Request.Method == "GET" &&
        context.Request.Path.HasValue &&
        !context.Request.Path.Value.Contains("/api") &&
        !context.Request.Path.Value.Contains("."))
    {
        context.Request.Path = new PathString("/");
        await next();
    }
});

app.UseFileServer();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ScalextricArcBleProtocolExplorer.Hubs.ConnectionHub>("hubs/car-id");
    endpoints.MapHub<ScalextricArcBleProtocolExplorer.Hubs.ConnectionHub>("hubs/connection");
    endpoints.MapHub<ScalextricArcBleProtocolExplorer.Hubs.CommandHub>("hubs/command");
    endpoints.MapHub<ScalextricArcBleProtocolExplorer.Hubs.LogHub>("hubs/log");
    endpoints.MapHub<ScalextricArcBleProtocolExplorer.Hubs.SlotHub>("hubs/slot");
    endpoints.MapHub<ScalextricArcBleProtocolExplorer.Hubs.ThrottleHub>("hubs/throttle");
    endpoints.MapHub<ScalextricArcBleProtocolExplorer.Hubs.ThrottleHub>("hubs/throttle-profile");
    endpoints.MapHub<ScalextricArcBleProtocolExplorer.Hubs.TrackHub>("hubs/track");
});

app.Run();
