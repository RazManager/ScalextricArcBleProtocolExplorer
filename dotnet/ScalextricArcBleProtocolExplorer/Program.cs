using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});


builder.Services.AddSingleton(serviceProvider =>
    new ScalextricArcBleProtocolExplorer.Services.ScalextricArcState()
);
builder.Services.AddHostedService<ScalextricArcBleProtocolExplorer.Services.BluezMonitorService>();

builder.Services.AddControllers();

builder.Services.AddCors();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors(builder =>
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyMethod()
                   .AllowAnyHeader());
                   //.AllowCredentials());
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseFileServer();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
