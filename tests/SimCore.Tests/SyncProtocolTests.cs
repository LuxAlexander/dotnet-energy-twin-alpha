using SimCore.Contracts;

namespace SimCore.Tests;

public class SyncProtocolTests
{
    private static readonly TimeState State = new(
        Guid.NewGuid(), 2, 7, SimMode.Running, 60, new DateTime(2026, 4, 1, 12, 0, 0),
        new DateTimeOffset(2026, 10, 25, 8, 0, 0, TimeSpan.Zero));

    public static TheoryData<SyncMessage> Messages() =>
    [
        new HelloMessage("BAT-01", "Battery", SyncProtocol.Version),
        new StatesMessage([State, State with { Sequence = 8, Mode = SimMode.Paused }]),
        new PingMessage(42, new DateTime(2026, 4, 1, 12, 0, 1), 0.6, "Synced"),
        new PingMessage(43, null, null, null),
        new PongMessage(42, new DateTimeOffset(2026, 10, 25, 8, 0, 0, 500, TimeSpan.Zero)),
        new ByeMessage("Simulator beendet")
    ];

    [Theory]
    [MemberData(nameof(Messages))]
    public void Message_SurvivesRoundTrip(SyncMessage message)
    {
        string line = SyncProtocol.Serialize(message);
        var back = SyncProtocol.Deserialize(line);

        Assert.DoesNotContain('\n', line);
        Assert.Equal(SyncProtocol.Serialize(back), line);
        Assert.Equal(message.GetType(), back.GetType());
    }

    [Fact]
    public void Message_IsReadableJson()
    {
        string line = SyncProtocol.Serialize(new StatesMessage([State]));

        Assert.StartsWith("{\"type\":\"states\"", line);
        Assert.Contains("\"mode\":\"Running\"", line);
        Assert.Contains("\"simAnchor\":\"2026-04-01T12:00:00\"", line);
    }

    [Theory]
    [InlineData("kein json")]
    [InlineData("{}")]
    [InlineData("{\"type\":\"unbekannt\"}")]
    [InlineData("{\"type\":\"hello\",\"protocolVersion\":1}")]
    [InlineData("{\"type\":\"states\"}")]
    [InlineData("{\"type\":\"states\",\"states\":[{\"mode\":\"Running\",\"speed\":0}]}")]
    [InlineData("{\"type\":\"states\",\"states\":[{\"mode\":\"Fliegen\",\"speed\":1}]}")]
    [InlineData("null")]
    public void InvalidMessage_IsRejected(string line)
    {
        Assert.Throws<InvalidDataException>(() => SyncProtocol.Deserialize(line));
    }
}
