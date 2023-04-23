using System.Buffers;

namespace Core;

public sealed class MQTTPubCompPackage : MQTTPackageWithIdentifier
{
    public MQTTPubCompPackage() : base(MQTTCommand.PubComp)
    {

    }

    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public MQTTPubCompReasonCode ReasonCode { get; set; } = MQTTPubCompReasonCode.Success;

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
        base.DecodeBody (ref reader, context);
    }

    public override void Dispose()
    {
        ReasonCode = default;
        ReasonString = null;
        UserProperties = null;
        base.Dispose();
    }

    public override string ToString()
    {
        return $"PubComp: [PacketIdentifier={PacketIdentifier}] [ReasonCode={ReasonCode}]";
    }
}
