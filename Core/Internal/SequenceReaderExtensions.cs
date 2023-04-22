using SuperSocket.ProtoBase;
using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace Core;

internal static class SequenceReaderExtensions
{
    private static bool TryReadReverseEndianness(ref SequenceReader<byte> reader, out ushort value)
    {
        if (reader.TryRead(out value))
        {
            value = BinaryPrimitives.ReverseEndianness(value);
            return true;
        }

        return false;
    }

    public static bool TryReadBigEndian(this ref SequenceReader<byte> reader, out ushort value)
    {
        if (!BitConverter.IsLittleEndian)
            return reader.TryRead(out value);

        return TryReadReverseEndianness(ref reader, out value);
    }

    public static byte[] ReadBinaryData(this ref SequenceReader<byte> reader)
    {
        reader.TryReadBigEndian(out ushort binaryDataLength);
        return reader.UnreadSequence.Slice(0, binaryDataLength).ToArray();
    }

    public static string ReadLengthEncodedString(this ref SequenceReader<byte> reader)
    {
        reader.TryReadBigEndian(out ushort length);

        if (length == 0)
            return string.Empty;

        return reader.ReadString(length, Encoding.UTF8);
    }

    public static string ReadString(this ref SequenceReader<byte> reader, int length, Encoding encoding)
    {
        var buffer = reader.Sequence.Slice(reader.Consumed, length);

        try
        {
            return buffer.GetString(encoding);
        }
        finally
        {
            reader.Advance(length);
        }
    }
}
