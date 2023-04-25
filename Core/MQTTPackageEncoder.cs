using SuperSocket.ProtoBase;
using System.Buffers;

namespace Core;

public sealed class MQTTPackageEncoder : IPackageEncoder<MQTTPackage>
{
    private const int FixedHeaderSize = 1; //固定字节长度

    public int Encode(IBufferWriter<byte> writer, MQTTPackage pack)
    {
        var writerLength = FixedHeaderSize;

        #region 申请1个固定头字节

        var fixedHeaderSpan = writer.GetSpan(FixedHeaderSize);
        writer.Advance(FixedHeaderSize);

        #endregion

        #region 写入可变长度 最长4字节最短一字节

        var variablePartSize = pack.CalculateSize();
        writerLength += writer.WriteVariable(variablePartSize);

        #endregion

        #region 写入包内容

        writerLength += pack.EncodeBody(writer);

        #endregion

        #region 写入fixedHeader

        var fixedHeader = pack.BuildFixedHeader();
        fixedHeaderSpan[0] = fixedHeader;

        #endregion

        return writerLength;
    }
}