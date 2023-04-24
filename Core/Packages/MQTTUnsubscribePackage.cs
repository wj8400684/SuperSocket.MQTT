using System.Buffers;

namespace Core;

public sealed class MQTTUnsubscribePackage : MQTTPackageWithIdentifier
{
    private const byte DefaultFixedHeader = 0x02;

    public MQTTUnsubscribePackage() : base(MQTTCommand.Unsubscribe)
    {
        FixedHeader = DefaultFixedHeader;
    }

    public List<string> TopicFilters { get; set; } = new();

    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public List<MQTTUserProperty>? UserProperties { get; set; }

    public override int CalculateSize()
    {
        const int StringSize = 2;

        if (!TopicFilters.Any())
            throw new MQTTProtocolViolationException("At least one topic filter must be set [MQTT-3.10.3-2].");

        var size = base.CalculateSize();

        foreach (var topic in TopicFilters)
            size += StringSize + topic.AsSpan().Length;

        return size;
    }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        if (!TopicFilters.Any())
            throw new MQTTProtocolViolationException("At least one topic filter must be set [MQTT-3.10.3-2].");

        var length = base.EncodeBody(writer);

        foreach (var topic in TopicFilters)
            length += writer.WriteEncoderString(topic);

        return length;
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        base.DecodeBody(ref reader, context);

        while (!reader.End)
        {
            var topic = reader.ReadEncoderString();

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
