using System.Buffers;

namespace Core;

public sealed class MQTTPingPackage : MQTTPackage
{
    public MQTTPingPackage() : base(MQTTCommand.Ping)
    {
    }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        return 0;
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
    }

    public override string ToString()
    {
        return "PingReq";
    }
}
