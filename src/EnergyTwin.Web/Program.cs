using EnergyTwin.Web.Components;
using EnergyTwin.Web.Presentation;

var builder = WebApplication.CreateBuilder(args);
// Auch dotnet run ohne Entwicklungsprofil muss die lokalen Blazor-Dateien finden.
builder.WebHost.UseStaticWebAssets();
var port = builder.Configuration.GetValue("Web:Port", 5180);
if (port is < 1024 or > 65535)
    throw new InvalidOperationException("Web:Port muss zwischen 1024 und 65535 liegen.");

builder.WebHost.ConfigureKestrel(options => options.ListenLocalhost(port));
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
var trends = new EnergyTwin.Web.Live.LiveTrends(builder.Configuration.GetValue("Live:History:MaxPoints", 720));
builder.Services.AddSingleton(trends);
builder.Services.AddSingleton(_ =>
{
    var store = new EnergyTwin.Web.Live.LiveStore();
    store.OnValidMessage = trends.Record;
    return store;
});
var instancePath = Path.Combine(builder.Environment.ContentRootPath,
    builder.Configuration["Instances:File"] ?? "instances.local.json");
builder.Services.AddSingleton(new EnergyTwin.Web.Management.InstanceStore(instancePath, port));
builder.Services.AddSingleton<EnergyTwin.Web.Management.ProcessManager>();
builder.Services.AddHostedService<EnergyTwin.Web.Live.MqttReceiver>();
builder.Services.AddSingleton<PreviewCatalog>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/fehler", createScopeForErrors: true);

app.UseStatusCodePagesWithReExecute("/nicht-gefunden", createScopeForStatusCodePages: true);
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
await app.RunAsync();
