using System.Buffers;

namespace Core;

public abstract class MQTTPackageWithIdentifier : MQTTPackage
{
    protected MQTTPackageWithIdentifier(MQTTCommand key) : base(key)
    {
    }

    public ushort PacketIdentifier { get; set; }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        if (PacketIdentifier == 0)
            throw new MQTTProtocolViolationException("PubAck packet has no packet identifier.");

        return writer.WriteBigEndian(PacketIdentifier);
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        reader.TryReadBigEndian(out var packetIdentifier);
        PacketIdentifier = packetIdentifier;
    }

    public override void Dispose()
    {
        PacketIdentifier = default;
        base.Dispose();
    }
}
