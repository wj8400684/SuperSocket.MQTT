using SuperSocket.ProtoBase;
using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace Core;

internal static class SequenceReaderExtensions
{
    public static bool TryReadBigEndian(this ref SequenceReader<byte> reader, out ushort value)
    {
        const int length = sizeof(ushort);

        value = 0;

        try
        {
            var span = reader.UnreadSpan[..length];
            value = BinaryPrimitives.ReadUInt16BigEndian(span);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            reader.Advance(length);
        }
    }

    public static byte[] ReadBinaryData(this ref SequenceReader<byte> reader)
    {
        reader.TryReadBigEndian(out ushort binaryDataLength);
        return reader.UnreadSequence.Slice(0, binaryDataLength).ToArray();
    }

    public static string ReadEncoderString(this ref SequenceReader<byte> reader)
    {
        reader.TryReadBigEndian(out ushort length);

        return length == 0 ? string.Empty : reader.ReadString(length, Encoding.UTF8);
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
