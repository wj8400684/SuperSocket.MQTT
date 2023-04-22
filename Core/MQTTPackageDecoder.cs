﻿using Package;
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

        var packetFactory = _packageFactoryPool.Get(command);

        var packet = packetFactory.Create();

        while (true)
        {
            if (!reader.TryRead(out byte lenByte))
                break;

            int lenSize = +1;
            if ((lenByte & 0x80) != 0x80)
                break;

            if (lenSize == 3)
                break;
        }

        packet.DecodeBody(ref reader, context);
        return packet;
    }
}