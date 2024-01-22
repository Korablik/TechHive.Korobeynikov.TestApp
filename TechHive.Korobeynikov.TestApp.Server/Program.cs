using Serilog;
using TechHive.Korobeynikov.TestApp.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

builder.Services.AddAppServices(config);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

#region // Web Sockets 
var endpoint = config.GetValue<PathString>("AppSettings:WebSocketEndpoint");
var wsOptions = new WebSocketOptions();
wsOptions.AllowedOrigins.Add(config.GetValue<string>("AppSettings:AllowedOrigins"));

app.UseWebSockets(wsOptions);
app.UseWebSocketPathVerification(endpoint);
app.UseWebSocketsBasicAuthorization();
app.UseWebSocketMiddleware(endpoint);
#endregion

app.Lifetime.ApplicationStarted.Register(OnStarted);
app.Lifetime.ApplicationStopping.Register(OnStopping);
app.Lifetime.ApplicationStopping.Register(OnStopped);

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/ping", async context =>
    {
        await context.Response.WriteAsync("pong");
    });
});

app.Run();

void OnStarted()
{
    Log.Information("OCPP Server Started");
}
void OnStopping()
{
    Log.Information("OCPP Server Stopping");
}
void OnStopped()
{
    Log.Information("OCPP Server Stopped");
    Log.CloseAndFlush();
}