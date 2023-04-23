using System.Buffers;

namespace Core;

public sealed class MQTTSubscribePackage : MQTTPackageWithIdentifier
{
    public MQTTSubscribePackage() : base(MQTTCommand.Subscribe)
    {
    }

    /// <summary>
    ///     It is a Protocol Error if the Subscription Identifier has a value of 0.
    /// </summary>
    public uint SubscriptionIdentifier { get; set; }

    public List<MQTTTopicFilter> TopicFilters { get; set; } = new();

    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public List<MQTTUserProperty>? UserProperties { get; set; }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        if (!TopicFilters.Any())
            throw new MQTTProtocolViolationException("At least one topic filter must be set [MQTT-3.8.3-3].");

        var length = base.EncodeBody(writer);

        foreach (var topicFilter in TopicFilters)
        {
            length += writer.WriteLengthEncodedString(topicFilter.Topic);
            length += writer.Write((byte)topicFilter.QualityOfServiceLevel);
        }

        return length;
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        base.DecodeBody(ref reader, context);

        while (!reader.End)
        {
            var topic = reader.ReadLengthEncodedString();
            reader.TryRead(out var qualityOfServiceLevel);

            TopicFilters.Add(new MQTTTopicFilter
            {
                Topic = topic,
                QualityOfServiceLevel = (MQTTQualityOfServiceLevel)qualityOfServiceLevel,
            });
        }
    }

    public override void Dispose()
    {
        TopicFilters.Clear();
        SubscriptionIdentifier = default;
        UserProperties = default;
        base.Dispose();
    }
}
