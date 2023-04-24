using SuperSocket.ProtoBase;
using System.Buffers;

namespace Core;

public sealed class MQTTPackageDecoder : IPackageDecoder<MQTTPackage>
{
    private readonly IMQTTPackageFactoryPool _packageFactoryPool;

    public MQTTPackageDecoder(IMQTTPackageFactoryPool packageFactoryPool)
    {
        _packageFactoryPool = packageFactoryPool;
    }

    public MQTTPackage Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        var reader = new SequenceReader<byte>(buffer);

        reader.TryRead(out var fixedHeader);//固定字节

        var command = (MQTTCommand)(fixedHeader >> 4);

        var packageFactory = _packageFactoryPool.Get(command);

        var package = packageFactory.Create();
        
        package.FixedHeader = fixedHeader;

        //可变字节
        var lenSize = 0;

        while (true)
        {
            if (!reader.TryRead(out byte lenByte))
                break;

            lenSize = +1;

            if ((lenByte & 0x80) != 0x80)
                break;

            if (lenSize == 3)
                break;
        }

        package.DecodeBody(ref reader, context);
        return package;
    }
}
