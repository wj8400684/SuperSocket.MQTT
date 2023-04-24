using SuperSocket.ProtoBase;
using System.Buffers;

namespace Core;

public sealed class MQTTPackageEncoder : IPackageEncoder<MQTTPackage>
{
    private readonly MQTTPublishPackageEncoder _publishPackageEncoder = new();
    private readonly MQTTDefaultPackageEncoder _defaultPackageEncoder = new();

    public int Encode(IBufferWriter<byte> writer, MQTTPackage pack)
    {
        //return _defaultPackageEncoder.Encode(writer, pack);
        return _publishPackageEncoder.Encode(writer, pack);
        if (pack is MQTTPublishPackage publishPackage)
        {
            return _publishPackageEncoder.Encode(writer, publishPackage);
        }
        else
        {
            return _defaultPackageEncoder.Encode(writer, pack);
        }
    }
}

internal sealed class MQTTPublishPackageEncoder : IPackageEncoder<MQTTPackage>
{
    private const int HeaderSize = 2; //头部字节长度
    private const int FixedHeaderSize = 1; //固定字节长度
    private const uint VariableByteIntegerMaxValue = 268435455;

    public int Encode(IBufferWriter<byte> writer, MQTTPackage pack)
    {
        #region 申请头部5个字节缓冲期 固定1个字节加上4个可变字节
        
        var headerSpan = writer.GetSpan(HeaderSize);
        writer.Advance(HeaderSize);

        #endregion

        #region 写入包内容

        var bodyLength = pack.EncodeBody(writer);

        #endregion

        #region 构建fixedHeader

        var fixedHeader = pack.BuildFixedHeader();

        #endregion

        #region 写入fixedHeader

        var remainingLengthSize = GetVariableByteIntegerSize((uint)bodyLength); //remainingLengthSize=4
        var headerSize = FixedHeaderSize + remainingLengthSize; //固定长度1+可变长度 headerSize=5
        var headerOffset = HeaderSize - headerSize; //headerOffset=3

        headerSpan[headerOffset] = fixedHeader;
        
        #endregion

        #region 固定字节直接赋值

        switch (bodyLength)
        {
            case 0:
                headerOffset += 1;
                headerSpan[headerOffset] = 0;
                return HeaderSize + bodyLength;
            case <= 127:
                headerOffset += 1;
                headerSpan[headerOffset] = (byte)bodyLength;
                return HeaderSize + bodyLength;
        }

        #endregion

        #region 可变字节

        if (bodyLength > VariableByteIntegerMaxValue) //最大不能超过256m
            throw new MQTTProtocolViolationException(
                $"The specified value ({bodyLength}) is too large for a variable byte integer.");

        var size = 0;
        var x = bodyLength;
        do
        {
            var encodedByte = x % 128;
            x /= 128;
            if (x > 0)
                encodedByte |= 128;

            headerSpan[headerOffset + size] = (byte)encodedByte;
            size++;
        } while (x > 0);

        #endregion
        
        return HeaderSize + bodyLength;
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

internal sealed class MQTTDefaultPackageEncoder : IPackageEncoder<MQTTPackage>
{
    private const int HeaderSize = 2; //头部字节长度

    public int Encode(IBufferWriter<byte> writer, MQTTPackage pack)
    {
        #region 申请缓冲区

        var headerSpan = writer.GetSpan(HeaderSize);
        writer.Advance(HeaderSize);

        #endregion

        #region 写入包内容

        var bodyLength = pack.EncodeBody(writer);

        #endregion

        #region 构建fixedHeader

        var fixedHeader = pack.BuildFixedHeader();

        #endregion

        headerSpan[0] = fixedHeader;
        headerSpan[1] = (byte)bodyLength;

        return HeaderSize + bodyLength;
    }
}