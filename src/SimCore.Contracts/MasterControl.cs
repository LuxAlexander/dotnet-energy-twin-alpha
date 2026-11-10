using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimCore.Contracts;

/// <summary>MQTT-Vertrag für bestätigte Master-Befehle.</summary>
public static class MasterControl
{
    public const string StatusTopic = "energytwin/master/status";
    public const string CommandTopic = "energytwin/master/commands";
    public const string ResultTopic = "energytwin/master/results";
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };
}

public sealed record MasterStatus(DateTimeOffset SentAt, DateTime SimTime, TimeState Current, TimeState Latest);
public sealed record MasterCommand(Guid CommandId, Guid MasterSession, long ExpectedSequence,
    string Action, DateTimeOffset ExpiresAt, double? Speed = null, DateTime? StartTime = null);
public sealed record MasterCommandResult(Guid CommandId, Guid MasterSession, bool Accepted, string Message, TimeState State);
