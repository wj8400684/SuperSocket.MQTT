using System.Buffers;
using SuperSocket.ProtoBase;

namespace Core;

public abstract class MQTTPackage : IKeyedPackageInfo<MQTTCommand>, IDisposable
{
    private IMQTTPackageFactory? _packetFactory;

    public MQTTCommand Key { get; }

    protected MQTTPackage(MQTTCommand key)
    {
        Key = key;
    }

    public virtual void Initialization(IMQTTPackageFactory factory)
    {
        _packetFactory = factory;
    }

    protected internal abstract void DecodeBody(ref SequenceReader<byte> reader, object context);

    public abstract int EncodeBody(IBufferWriter<byte> writer);

    public virtual void Dispose()
    {
        _packetFactory?.Return(this);
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, GetType());
    }
}