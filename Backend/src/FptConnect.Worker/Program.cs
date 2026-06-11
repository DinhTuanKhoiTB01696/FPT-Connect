using FptConnect.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<OutboxWorker>();
var host = builder.Build();
host.Run();
