using System.Buffers;
using System.Runtime.CompilerServices;
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

    #region protected

    protected internal abstract void DecodeBody(ref SequenceReader<byte> reader, object context);

    #endregion

    #region public

    public abstract int EncodeBody(IBufferWriter<byte> writer);

    public virtual int CalculateSize()
    {
        return 0;//FixedHeader
    }

    public virtual void Dispose()
    {
        _packetFactory?.Return(this);
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, GetType());
    }

    #endregion

    #region internal

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

    #endregion
}