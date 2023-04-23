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

        reader.TryRead(out byte firstByte);

        var command = (MQTTCommand)(firstByte >> 4);

        var packageFactory = _packageFactoryPool.Get(command);

        var package = packageFactory.Create();
        package.FixedHeader = firstByte;

        int lenSize;

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
