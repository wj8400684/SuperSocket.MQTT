using System.Buffers;

namespace Core;

public sealed class MQTTPublishPackage : MQTTPackageWithIdentifier
{
    public MQTTPublishPackage() : base(MQTTCommand.Publish)
    {
    }

    public string? ContentType { get; set; }

    public byte[]? CorrelationData { get; set; }

    public bool Dup { get; set; }

    public uint MessageExpiryInterval { get; set; }

    public MQTTPayloadFormatIndicator PayloadFormatIndicator { get; set; } = MQTTPayloadFormatIndicator.Unspecified;

    public ArraySegment<byte>? PayloadSegment { get; set; }

    public MQTTQualityOfServiceLevel QualityOfServiceLevel { get; set; } = MQTTQualityOfServiceLevel.AtMostOnce;

    public string? ResponseTopic { get; set; }

    public bool Retain { get; set; }

    public List<uint>? SubscriptionIdentifiers { get; set; }

    public string Topic { get; set; } = default!;

    public ushort TopicAlias { get; set; }

    public List<MQTTUserProperty>? UserProperties { get; set; }

    public override int CalculateSize()
    {
        const int StringSize = 2;

        var size = 0;

        if (!string.IsNullOrWhiteSpace(Topic))
        {
            size += StringSize;
            size += Topic.AsSpan().Length;
        }

        if (QualityOfServiceLevel > MQTTQualityOfServiceLevel.AtMostOnce)
            size += base.CalculateSize();

        if (PayloadSegment != null)
            size += PayloadSegment.Value.Count;

        return size;
    }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        var length = writer.WriteEncoderString(Topic);

        if (QualityOfServiceLevel > MQTTQualityOfServiceLevel.AtMostOnce)
        {
            //解析包id
            length += base.EncodeBody(writer);
        }
        else
        {
            if (PacketIdentifier > 0)
                throw new MQTTProtocolViolationException("Packet identifier must be empty if QoS == 0 [MQTT-2.3.1-5].");
        }

        // The payload is the past part of the packet. But it is not added here in order to keep
        // memory allocation low.

        byte fixedHeader = 0;

        if (Retain)
            fixedHeader |= 0x01;

        fixedHeader |= (byte)((byte)QualityOfServiceLevel << 1);

        if (Dup)
            fixedHeader |= 0x08;

        FixedHeader = fixedHeader;

        return length;
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        var retain = (FixedHeader & 0x1) > 0;
        var qualityOfServiceLevel = (MQTTQualityOfServiceLevel)((FixedHeader >> 1) & 0x3);
        var dup = (FixedHeader & 0x8) > 0;

        var topic = reader.ReadEncoderString();

        if (qualityOfServiceLevel > MQTTQualityOfServiceLevel.AtMostOnce)
            base.DecodeBody(ref reader, context);

        Retain = retain;
        Topic = topic;
        QualityOfServiceLevel = qualityOfServiceLevel;
        Dup = dup;

        if (reader.End)
            return;

        PayloadSegment = new ArraySegment<byte>(reader.UnreadSpan.ToArray());
    }

    public override void Dispose()
    {
        ContentType = default;
        CorrelationData = default;
        Dup = default;
        MessageExpiryInterval = default;
        PayloadFormatIndicator = default;
        PayloadSegment = default;
        QualityOfServiceLevel = default;
        ResponseTopic = default;
        Retain = default;
        SubscriptionIdentifiers = default;
        Topic = string.Empty;
        TopicAlias = default;
        UserProperties = default;
        base.Dispose();
    }

    public override string ToString()
    {
        return
            $"Publish: [Topic={Topic}] [PayloadLength={PayloadSegment?.Count}] [QoSLevel={QualityOfServiceLevel}] [Dup={Dup}] [Retain={Retain}] [PacketIdentifier={PacketIdentifier}]";
    }
}
