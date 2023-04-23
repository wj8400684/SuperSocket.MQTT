using System.Buffers;

namespace Core;

public sealed class MQTTConnectRespPackage : MQTTPackage
{
    public MQTTConnectRespPackage() : base(MQTTCommand.ConnAck)
    {
    }

    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public string? AssignedClientIdentifier { get; set; }

    public byte[]? AuthenticationData { get; set; }

    public string? AuthenticationMethod { get; set; }

    /// <summary>
    ///     Added in MQTTv3.1.1.
    /// </summary>
    public bool IsSessionPresent { get; set; }

    public uint MaximumPacketSize { get; set; }

    public MQTTQualityOfServiceLevel MaximumQoS { get; set; }

    /// <summary>
    ///     Added in MQTTv5.
    /// </summary>
    public MQTTConnectReasonCode ReasonCode { get; set; }

    public string? ReasonString { get; set; }

    public ushort ReceiveMaximum { get; set; }

    public string? ResponseInformation { get; set; }

    public bool RetainAvailable { get; set; }

    public MQTTConnectReturnCode ReturnCode { get; set; }

    public ushort ServerKeepAlive { get; set; }

    public string? ServerReference { get; set; }

    public uint SessionExpiryInterval { get; set; }

    public bool SharedSubscriptionAvailable { get; set; }

    public bool SubscriptionIdentifiersAvailable { get; set; }

    public ushort TopicAliasMaximum { get; set; }

    public List<MQTTUserProperty>? UserProperties { get; set; }

    public bool WildcardSubscriptionAvailable { get; set; }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        byte connectAcknowledgeFlags = 0x0;
        if (IsSessionPresent)
            connectAcknowledgeFlags |= 0x1;

        var length = writer.Write(connectAcknowledgeFlags);
        length += writer.Write((byte)ReturnCode);

        return length;
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        reader.TryRead(out var acknowledgeFlags);
        IsSessionPresent = (acknowledgeFlags & 0x1) > 0;

        reader.TryRead(out var returnCode);
        ReturnCode = (MQTTConnectReturnCode)returnCode;
    }

    public override void Dispose()
    {
        AssignedClientIdentifier = default;
        AuthenticationData = default;
        AuthenticationMethod = default;
        IsSessionPresent = default;
        MaximumPacketSize = default;
        MaximumQoS = default;
        ReasonCode = default;
        ReasonString = default;
        ReceiveMaximum = default;
        ResponseInformation = default;
        RetainAvailable = default;
        ReturnCode = default;
        ServerKeepAlive = default;
        ServerReference = default;
        SessionExpiryInterval = default;
        SharedSubscriptionAvailable = default;
        SubscriptionIdentifiersAvailable = default;
        TopicAliasMaximum = default;
        UserProperties = default;
        WildcardSubscriptionAvailable = default;
        base.Dispose();
    }
}