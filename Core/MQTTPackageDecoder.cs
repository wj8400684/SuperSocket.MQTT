using SuperSocket.ProtoBase;
using System.Buffers;

namespace Core;

public sealed class MQTTPackageDecoder : IPackageDecoder<MQTTPackage>
{
    private const int v1 = 0x80;

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

        //跳过可变字节
        for (var i = 0; i >= 3; i++)
        {
            if (!reader.TryRead(out byte encodedByte))
                break;

            if ((encodedByte & v1) != v1)
                break;
        }

        package.DecodeBody(ref reader, context);

        return package;
    }
}
