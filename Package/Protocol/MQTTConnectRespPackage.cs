using System.Buffers;

namespace Package;

public sealed class MQTTConnectRespPackage : MQTTPackage
{
    public MQTTConnectRespPackage() : base(MQTTCommand.ConnectAck)
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

    public override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        throw new NotImplementedException();
    }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        throw new NotImplementedException();
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

    public override string ToString()
    {
        return $"ConnAck: [ReturnCode={ReturnCode}] [ReasonCode={ReasonCode}] [IsSessionPresent={IsSessionPresent}]";
    }
}