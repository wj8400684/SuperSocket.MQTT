
using System.Buffers;

namespace Core;

public sealed class MQTTUnsubRespPackage : MQTTPackageWithIdentifier
{
    public MQTTUnsubRespPackage() : base(MQTTCommand.UnsubAck)
    {
    }

    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public List<MQTTUnsubscribeReasonCode>? ReasonCodes { get; set; }

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
        ReasonCodes = default;
        ReasonString = default;
        UserProperties = default;
        base.Dispose();
    }

    public override string ToString()
    {
        var reasonCodesText = string.Empty;
        if (ReasonCodes != null)
            reasonCodesText = string.Join(",", ReasonCodes?.Select(f => f.ToString()));

        return $"UnsubAck: [PacketIdentifier={PacketIdentifier}] [ReasonCodes={reasonCodesText}] [ReasonString={ReasonString}]";
    }
}
