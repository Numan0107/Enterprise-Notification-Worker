using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EnterpriseNotificationWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<NotificationQueue>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
