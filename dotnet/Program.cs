using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});


builder.Services.AddSingleton(serviceProvider =>
    new ScalextricArcBleProtocolExplorer.Services.ScalextricArcState
    (
        serviceProvider.GetRequiredService<IHubContext<ScalextricArcBleProtocolExplorer.Hubs.ThrottleHub, ScalextricArcBleProtocolExplorer.Hubs.IThrottleHub>>()
    )
);

builder.Services.AddScoped<ScalextricArcBleProtocolExplorer.Services.CpuInfo.ICpuInfoService, ScalextricArcBleProtocolExplorer.Services.CpuInfo.CpuInfoService>();

builder.Services.AddHostedService<ScalextricArcBleProtocolExplorer.Services.BluezMonitorService>();

builder.Services.AddControllers();

builder.Services.AddSignalR()
    .AddJsonProtocol(options => {
        options.PayloadSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddCors();

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
    endpoints.MapHub<ScalextricArcBleProtocolExplorer.Hubs.ThrottleHub>("hubs/throttle");
});

app.Run();
