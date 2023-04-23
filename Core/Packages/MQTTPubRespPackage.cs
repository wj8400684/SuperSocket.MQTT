using System.Buffers;

namespace Core;

public sealed class MQTTPubRespPackage : MQTTPackageWithIdentifier
{
    public MQTTPubRespPackage() : base(MQTTCommand.PubAck)
    {

    }

    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public MQTTPubAckReasonCode ReasonCode { get; set; } = MQTTPubAckReasonCode.Success;

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
        return $"PubAck: [PacketIdentifier={PacketIdentifier}] [ReasonCode={ReasonCode}]";
    }
}
