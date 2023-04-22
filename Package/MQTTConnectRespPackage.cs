using System.Buffers;

namespace Package;

public sealed class MQTTConnectRespPackage : MQTTPackage
{
    public MQTTConnectRespPackage() : base(MQTTCommand.ConnectAck)
    {
    }
    
    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        throw new NotImplementedException();
    }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}