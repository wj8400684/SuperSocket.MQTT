using System.Buffers;

namespace Core;

public sealed class MQTTUnsubscribePackage : MQTTPackageWithIdentifier
{
    public MQTTUnsubscribePackage() : base(MQTTCommand.Unsubscribe)
    {
    }

    public List<string> TopicFilters { get; set; } = new();

    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public List<MQTTUserProperty>? UserProperties { get; set; }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        if (!TopicFilters.Any())
            throw new MQTTProtocolViolationException("At least one topic filter must be set [MQTT-3.10.3-2].");

        var length = base.EncodeBody(writer);

        if (!TopicFilters.Any())
            return length;

        foreach (var topic in TopicFilters)
            length += writer.WriteLengthEncodedString(topic);

        return length;
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        base.DecodeBody(ref reader, context);

        while (!reader.End)
        {
            var topic = reader.ReadLengthEncodedString();

            TopicFilters.Add(topic);
        }
    }

    public override void Dispose()
    {
        TopicFilters.Clear();
        UserProperties = default;
        base.Dispose();
    }

    public override string ToString()
    {
        var topicFiltersText = string.Join(",", TopicFilters);
        return $"Unsubscribe: [PacketIdentifier={PacketIdentifier}] [TopicFilters={topicFiltersText}]";
    }
}
