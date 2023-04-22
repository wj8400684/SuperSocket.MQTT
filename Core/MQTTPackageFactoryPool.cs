using Core.Packages;
using Package;

namespace Core;

public sealed class MQTTPackageFactoryPool : IMQTTPackageFactoryPool
{
    private readonly IMQTTPackageFactory[] _packetFactories;

    public MQTTPackageFactoryPool()
    {
        _packetFactories = new IMQTTPackageFactory[20];

        _packetFactories[(byte)MQTTCommand.Connect] = new MQTTPackageFactory<MQTTConnectPackage>();
        _packetFactories[(byte)MQTTCommand.ConnectAck] = new MQTTPackageFactory<MQTTConnectRespPackage>();
    }

    public IMQTTPackageFactory Get(MQTTCommand command)
    {
        throw new NotImplementedException();
    }

    public IMQTTPackageFactory Get<TPackage>()
    {
        throw new NotImplementedException();
    }
}
