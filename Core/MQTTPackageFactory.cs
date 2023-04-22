using Package;
using System.Collections.Concurrent;

namespace Core;

public sealed class MQTTPackageFactory<TPackage> : 
    IMQTTPackageFactory where TPackage : 
    MQTTPackage, new ()
{
    private const int DefaultMaxCount = 10;
    private readonly ConcurrentQueue<MQTTPackage> _packagePool = new();

    public MQTTPackageFactory()
    {
        for (int i = 0; i < DefaultMaxCount; i++)
        {
            var packet = new TPackage();

            packet.Initialization(this);

            _packagePool.Enqueue(packet);
        }
    }

    public MQTTPackage Create()
    {
        if (_packagePool.TryDequeue(out var package))
            return package;

        var packet = new TPackage();

        packet.Initialization(this);

        return packet;
    }

    public void Return(MQTTPackage package)
    {
        _packagePool.Enqueue(package);
    }
}
