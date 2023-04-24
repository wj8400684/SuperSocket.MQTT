using Core.Protocol;
using SuperSocket.ProtoBase;
using System.Buffers;
using System.Buffers.Binary;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Core;

internal static class BufferWriterExtensions
{
    /// <summary>
    /// 写入字节
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Write(this IBufferWriter<byte> writer, byte value)
    {
        const int size = 1;

        var span = writer.GetSpan(size);
        span[0] = value;
        writer.Advance(size);

        return size;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="propertyId"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteAsFourByteInteger(this IBufferWriter<byte> writer, MQTTPropertyId propertyId, uint value)
    {
        var length = writer.Write((byte)propertyId);
        length += writer.Write((byte)(value >> 24));
        length += writer.Write((byte)(value >> 16));
        length += writer.Write((byte)(value >> 8));
        length += writer.Write((byte)value);

        return length;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBinaryData(this IBufferWriter<byte> writer, byte[] value)
    {
        var span = writer.GetSpan(value.Length);
        value.CopyTo(span);
        writer.Advance(span.Length);

        return span.Length;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteSessionExpiryInterval(this IBufferWriter<byte> writer, uint value)
    {
        return writer.WriteAsFourByteInteger(MQTTPropertyId.SessionExpiryInterval, value);
    }

    /// <summary>
    /// 写入uint16
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBigEndian(this IBufferWriter<byte> writer, ushort value)
    {
        const int size = sizeof(ushort);

        var span = writer.GetSpan(size);
        BinaryPrimitives.WriteUInt16BigEndian(span, value);
        writer.Advance(size);

        return size;
    }

    /// <summary>
    /// 写入可变字节
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteVariable(this IBufferWriter<byte> writer, int value)
    {
        const int v1 = 128;
        const int v2 = 0x80;
        var length = 0;

        do
        {
            var encodedByte = value % v1;

            value /= v1;
            if (value > 0)
                encodedByte |= v2;

            length++;
            writer.Write((byte)encodedByte);
        }
        while (value > 0);

        return length;
    }

    /// <summary>
    /// 写入带有字符串长度的字符串
    /// (ushort)bodyLength body
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteEncoderString(this IBufferWriter<byte> writer, string value)
    {
        const int size = sizeof(ushort);

        //申请2子字节缓冲区
        var buffer = writer.GetSpan(size);
        
        //跳过
        writer.Advance(size);

        //写入字符串
        var length = writer.Write(value, Encoding.UTF8);

        //写入字符串长度
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)length);

        return size + length;
    }
}
