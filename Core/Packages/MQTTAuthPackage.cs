using System.Buffers;

namespace Core;

/// <summary>
/// Added in MQTTv5.0.0.
/// </summary>
public sealed class MQTTAuthPackage : MQTTPackage
{
    public MQTTAuthPackage() : base(MQTTCommand.Auth)
    {
    }

    public byte[]? AuthenticationData { get; set; }

    public string? AuthenticationMethod { get; set; }

    public MQTTAuthenticateReasonCode ReasonCode { get; set; }

    public string? ReasonString { get; set; }

    public List<MQTTUserProperty>? UserProperties { get; set; }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        throw new NotImplementedException();
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        AuthenticationData = default;
        AuthenticationMethod = default;
        ReasonCode = default;
        ReasonString = default;
        UserProperties = default;
        base.Dispose();
    }
}
