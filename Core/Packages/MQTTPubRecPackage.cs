using System.Buffers;

namespace Core;

public sealed class MQTTPubRecPackage : MQTTPackageWithIdentifier
{
    public MQTTPubRecPackage() : base(MQTTCommand.PubRec)
    {

    }
    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public MQTTPubRecReasonCode ReasonCode { get; set; } = MQTTPubRecReasonCode.Success;

    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public string? ReasonString { get; set; }

    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public List<MQTTUserProperty>? UserProperties { get; set; }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        return base.EncodeBody(writer);
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        base.DecodeBody(ref reader, context);
    }

    public override void Dispose()
    {
        ReasonCode = default;
        ReasonString = default;
        UserProperties = default;
        base.Dispose();
    }

    public override string ToString()
    {
        return $"PubRec: [PacketIdentifier={PacketIdentifier}] [ReasonCode={ReasonCode}]";
    }
}
