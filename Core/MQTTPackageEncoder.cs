using SuperSocket.ProtoBase;
using System.Buffers;

namespace Core;

public sealed class MQTTPackageEncoder : IPackageEncoder<MQTTPackage>
{
    private const int FixedHeaderSize = 1;

    public int Encode(IBufferWriter<byte> writer, MQTTPackage pack)
    {
        var fixedHeader = (int)pack.Key << 4;
        fixedHeader |= pack.FixedHeader;

        writer.Write((byte)fixedHeader);

        while (true)
        {

        }


        var length = pack.EncodeBody(writer);

        return length;
    }

    private static int GetVariableByteIntegerSize(uint value)
    {
        // From RFC: Table 2.4 Size of Remaining Length field

        // 0 (0x00) to 127 (0x7F)
        if (value <= 127)
            return 1;

        // 128 (0x80, 0x01) to 16 383 (0xFF, 0x7F)
        if (value <= 16383)
            return 2;

        // 16 384 (0x80, 0x80, 0x01) to 2 097 151 (0xFF, 0xFF, 0x7F)
        if (value <= 2097151)
            return 3;

        // 2 097 152 (0x80, 0x80, 0x80, 0x01) to 268 435 455 (0xFF, 0xFF, 0xFF, 0x7F)
        return 4;
    }
}
