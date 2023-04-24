using System.Buffers;

namespace Core;

public sealed class MQTTSubRespPackage : MQTTPackageWithIdentifier
{
    public MQTTSubRespPackage() : base(MQTTCommand.SubAck)
    {
    }

    /// <summary>
    ///     Reason Code is used in MQTTv5.0.0 and backward compatible to v.3.1.1. Return Code is used in MQTTv3.1.1
    /// </summary>
    public List<MQTTSubscribeReasonCode>? ReasonCodes { get; set; }

    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public string? ReasonString { get; set; }

    /// <summary>
    /// Added in MQTTv5.
    /// </summary>
    public List<MQTTUserProperty>? UserProperties { get; set; }

    public override int CalculateSize()
    {
        var size = base.CalculateSize();

        if (ReasonCodes == null || !ReasonCodes.Any())
            return size;

        return size + ReasonCodes.Count;
    }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        var length = base.EncodeBody(writer);

        if (ReasonCodes == null || !ReasonCodes.Any())
            return length;

        foreach (var reasonCode in ReasonCodes)
        {
            var packetSubscribeReturnCode = reasonCode switch
            {
                MQTTSubscribeReasonCode.GrantedQoS0 => MQTTSubscribeReturnCode.SuccessMaximumQoS0,
                MQTTSubscribeReasonCode.GrantedQoS1 => MQTTSubscribeReturnCode.SuccessMaximumQoS1,
                MQTTSubscribeReasonCode.GrantedQoS2 => MQTTSubscribeReturnCode.SuccessMaximumQoS2,
                _ => MQTTSubscribeReturnCode.Failure,
            };

            length += writer.Write((byte)packetSubscribeReturnCode);
        }

        return length;
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        base.DecodeBody(ref reader, context);

        ReasonCodes = new List<MQTTSubscribeReasonCode>((int)reader.Remaining);

        while (reader.TryRead(out var reasonCodes))
            ReasonCodes.Add((MQTTSubscribeReasonCode)reasonCodes);
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
        var reasonCodesText = string.Join(",", ReasonCodes?.Select(f => f.ToString()));

        return $"SubAck: [PacketIdentifier={PacketIdentifier}] [ReasonCode={reasonCodesText}]";
    }
}
