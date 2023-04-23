﻿using Core.Protocol;
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
    /// 写入字符串
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteLengthEncodedString(this IBufferWriter<byte> writer, string value)
    {
        const int size = sizeof(ushort);

        var buffer = writer.GetSpan(size);
        writer.Advance(size);

        var length = writer.Write(value, Encoding.UTF8);

        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)length);

        return size + length;
    }
}
