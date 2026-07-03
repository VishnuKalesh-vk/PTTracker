using PTTracker.Application;
using PTTracker.Infrastructure;
using PTTracker.Worker;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((_, loggerConfig) =>
    loggerConfig
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console());

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
