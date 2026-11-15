using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Devices.Common.Control;

public sealed record DeviceRegistration(string Id, string Kind, Guid Session, int Port, string Token)
{
    [System.Text.Json.Serialization.JsonIgnore]
    public string? FilePath { get; init; }
}
public sealed record DeviceCommand(string Action, string? Value = null);
public sealed record DeviceReply<T>(T State, string? Error = null);

public static class DeviceControl
{
    public static string DirectoryPath => Environment.GetEnvironmentVariable("ENERGYTWIN_DEVICE_CONTROL_DIR") ?? Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EnergyTwin", "device-control");

    public static IEnumerable<DeviceRegistration> Discover(string? directory = null)
    {
        directory ??= DirectoryPath;
        if (!Directory.Exists(directory)) yield break;
        foreach (var file in Directory.EnumerateFiles(directory, "*.json"))
        {
            DeviceRegistration? registration;
            try { registration = JsonSerializer.Deserialize<DeviceRegistration>(File.ReadAllText(file)); }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException) { continue; }
            if (registration is { Port: > 0 and <= 65535, Token.Length: 64 }
                && registration.Token.All(Uri.IsHexDigit)
                && string.Equals(Path.GetFileName(file), $"{registration.Session}.json", StringComparison.OrdinalIgnoreCase))
                yield return registration with { FilePath = file };
        }
    }

    public static async Task<DeviceReply<T>?> RequestAsync<T>(HttpClient client, DeviceRegistration device,
        DeviceCommand? command, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(command is null ? HttpMethod.Get : HttpMethod.Post,
            $"http://127.0.0.1:{device.Port}/control/{device.Session}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", device.Token);
        if (command is not null) request.Content = JsonContent.Create(command);
        using var response = await client.SendAsync(request, cancellationToken);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<DeviceReply<T>>(cancellationToken)
            : null;
    }

    public static async Task<IAsyncDisposable> StartAsync<T>(string id, string kind,
        Func<DeviceCommand?, CancellationToken, Task<DeviceReply<T>>> dispatchAsync, CancellationToken cancellationToken,
        int? containerPort = null)
    {
        var session = Guid.NewGuid();
        string token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var builder = WebApplication.CreateSlimBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(containerPort is null ? IPAddress.Loopback : IPAddress.Any, containerPort ?? 0);
            options.Limits.MaxRequestBodySize = 4096;
        });
        var app = builder.Build();
        app.MapMethods("/control/{sessionId:guid}", ["GET", "POST"], async (HttpContext context, Guid sessionId) =>
        {
            if (sessionId != session || !string.Equals(context.Request.Headers.Authorization, $"Bearer {token}", StringComparison.Ordinal))
                return Results.Unauthorized();
            var command = HttpMethods.IsPost(context.Request.Method)
                ? await context.Request.ReadFromJsonAsync<DeviceCommand>(context.RequestAborted)
                : null;
            if (HttpMethods.IsPost(context.Request.Method) && command is null)
                return Results.BadRequest();
            return Results.Json(await dispatchAsync(command, context.RequestAborted));
        });

        Directory.CreateDirectory(DirectoryPath);
        var path = Path.Combine(DirectoryPath, $"{session}.json");
        try
        {
            await app.StartAsync(cancellationToken);
            var address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
            var registration = new DeviceRegistration(id, kind, session, new Uri(address).Port, token);
            var fileOptions = new FileStreamOptions { Mode = FileMode.CreateNew, Access = FileAccess.Write };
            if (!OperatingSystem.IsWindows())
                fileOptions.UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite;
            await using (var file = new FileStream(path + ".tmp", fileOptions))
                await JsonSerializer.SerializeAsync(file, registration, cancellationToken: cancellationToken);
            File.Move(path + ".tmp", path);
            return new ControlLifetime(app, path);
        }
        catch
        {
            await app.DisposeAsync();
            File.Delete(path + ".tmp");
            throw;
        }
    }

    public static bool IsRegistered(DeviceRegistration device) =>
        File.Exists(device.FilePath ?? Path.Combine(DirectoryPath, $"{device.Session}.json"));

    private sealed class ControlLifetime(WebApplication app, string path) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            try { File.Delete(path); }
            finally
            {
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                await app.StopAsync(timeout.Token);
                await app.DisposeAsync();
            }
        }
    }
}
