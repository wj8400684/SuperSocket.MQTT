using System.Buffers;
using SuperSocket.ProtoBase;

namespace Core;

public abstract class MQTTPackage : IKeyedPackageInfo<MQTTCommand>, IDisposable
{
    private IMQTTPackageFactory? _packetFactory;

    public MQTTCommand Key { get; }

    public byte FixedHeader { get; set; }

    protected MQTTPackage(MQTTCommand key)
    {
        Key = key;
    }

    protected internal abstract void DecodeBody(ref SequenceReader<byte> reader, object context);

    public abstract int EncodeBody(IBufferWriter<byte> writer);

    internal virtual byte BuildFixedHeader()
    {
        var fixedHeader = (int)Key << 4;
        fixedHeader |= FixedHeader;

        return (byte)fixedHeader;
    }
    
    internal virtual void Initialization(IMQTTPackageFactory factory)
    {
        _packetFactory = factory;
    }

    public virtual void Dispose()
    {
        FixedHeader = 0;
        _packetFactory?.Return(this);
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, GetType());
    }
}