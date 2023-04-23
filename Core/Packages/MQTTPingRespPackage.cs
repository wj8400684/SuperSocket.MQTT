using System.Buffers;

namespace Core;

public sealed class MQTTPingRespPackage : MQTTPackage
{
    public MQTTPingRespPackage() : base(MQTTCommand.PingAck)
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
        return "PingResp";
    }
}
