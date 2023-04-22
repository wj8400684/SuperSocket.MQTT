using Core;
using SuperSocket.ProtoBase;
using System.Buffers;

namespace Core;

public sealed class MQTTConnectPackage : MQTTPackage
{
    public MQTTConnectPackage() : base(MQTTCommand.Connect)
    {
    }

    public byte[]? AuthenticationData { get; set; }

    public string? AuthenticationMethod { get; set; }

    /// <summary>
    ///     Also called "Clean Start" in MQTTv5.
    /// </summary>
    public bool CleanSession { get; set; }

    public string? ClientId { get; set; }

    public byte[]? WillCorrelationData { get; set; }

    public ushort KeepAlivePeriod { get; set; }

    public uint MaximumPacketSize { get; set; }

    public byte[]? Password { get; set; }

    public ushort ReceiveMaximum { get; set; }

    public bool RequestProblemInformation { get; set; }

    public bool RequestResponseInformation { get; set; }

    public string? WillResponseTopic { get; set; }

    public uint SessionExpiryInterval { get; set; }

    public ushort TopicAliasMaximum { get; set; }

    public string? Username { get; set; }

    public List<MQTTUserProperty>? UserProperties { get; set; }

    public string? WillContentType { get; set; }

    public uint WillDelayInterval { get; set; }

    public bool WillFlag { get; set; }

    public byte[]? WillMessage { get; set; }

    public uint WillMessageExpiryInterval { get; set; }

    public MQTTPayloadFormatIndicator WillPayloadFormatIndicator { get; set; } = MQTTPayloadFormatIndicator.Unspecified;

    public MQTTQualityOfServiceLevel WillQoS { get; set; } = MQTTQualityOfServiceLevel.AtMostOnce;

    public bool WillRetain { get; set; }

    public string? WillTopic { get; set; }

    public List<MQTTUserProperty>? WillUserProperties { get; set; }

    public bool TryPrivate { get; set; }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        const string MQTT = "MQTT";
        const string MQIsdp = "MQIsdp";

        var protocolName = reader.ReadLengthEncodedString();

        if (protocolName != MQTT && protocolName != MQIsdp)
            throw new MQTTProtocolViolationException("MQTT protocol name do not match MQTT v3.");

        reader.TryRead(out byte protocolVersion);

        TryPrivate = (protocolVersion & 0x80) > 0;
        protocolVersion &= 0x7F;

        if (protocolVersion != 3 && protocolVersion != 4)
            throw new MQTTProtocolViolationException("MQTT protocol version do not match MQTT v3.");

        reader.TryRead(out byte connectFlags);

        if ((connectFlags & 0x1) > 0)
            throw new MQTTProtocolViolationException("The first bit of the Connect Flags must be set to 0.");

        CleanSession = (connectFlags & 0x2) > 0;

        reader.TryReadBigEndian(out ushort keepAlivePeriod);
        KeepAlivePeriod = keepAlivePeriod;
        ClientId = reader.ReadLengthEncodedString();

        var willFlag = (connectFlags & 0x4) > 0;
        var willQoS = (connectFlags & 0x18) >> 3;
        var willRetain = (connectFlags & 0x20) > 0;
        var passwordFlag = (connectFlags & 0x40) > 0;
        var usernameFlag = (connectFlags & 0x80) > 0;

        if (willFlag)
        {
            WillFlag = true;
            WillQoS = (MQTTQualityOfServiceLevel)willQoS;
            WillRetain = willRetain;

            WillTopic = reader.ReadLengthEncodedString();
            WillMessage = reader.ReadBinaryData();
        }

        if (usernameFlag)
            Username = reader.ReadLengthEncodedString();

        if (passwordFlag)
            Password = reader.ReadBinaryData();
    }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        AuthenticationData = default;
        AuthenticationMethod = default;
        CleanSession = default;
        ClientId = default;
        WillCorrelationData = default;
        KeepAlivePeriod = default;
        MaximumPacketSize = default;
        Password = default;
        ReceiveMaximum = default;
        RequestProblemInformation = default;
        RequestResponseInformation = default;
        WillResponseTopic = default;
        SessionExpiryInterval = default;
        TopicAliasMaximum = default;
        Username = default;
        UserProperties = default;
        WillContentType = default;
        WillDelayInterval = default;
        WillFlag = default;
        WillMessage = default;
        WillMessageExpiryInterval = default;
        WillPayloadFormatIndicator = default;
        WillQoS = default;
        WillRetain = default;
        WillTopic = default;
        WillUserProperties = default;
        TryPrivate = default;
        base.Dispose();
    }

    public override string ToString()
    {
        var passwordText = string.Empty;

        if (Password != null)
            passwordText = "****";

        return $"Connect: [ClientId={ClientId}] [Username={Username}] [Password={passwordText}] [KeepAlivePeriod={KeepAlivePeriod}] [CleanSession={CleanSession}]";
    }
}