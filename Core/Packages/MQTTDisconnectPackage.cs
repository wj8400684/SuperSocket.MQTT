using System.Buffers;

namespace Core;

public sealed class MQTTDisconnectPackage : MQTTPackage
{
    public MQTTDisconnectPackage() : base(MQTTCommand.Disconnect)
    {
    }

    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public MQTTDisconnectReasonCode ReasonCode { get; set; } = MQTTDisconnectReasonCode.NormalDisconnection;

    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public string? ReasonString { get; set; }

    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public string? ServerReference { get; set; }

    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public uint SessionExpiryInterval { get; set; }

    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public List<MQTTUserProperty>? UserProperties { get; set; }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        throw new MQTTProtocolViolationException($"Packet type ({Key}) not supported.");
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        throw new MQTTProtocolViolationException($"Packet type ({Key}) not supported.");
    }

    public override void Dispose()
    {
        ReasonCode = default;
        ReasonString = default;
        ServerReference = default;
        SessionExpiryInterval = default;
        UserProperties = default;
        base.Dispose();
    }

    public override string ToString()
    {
        return $"Disconnect: [ReasonCode={ReasonCode}]";
    }
}
