namespace Core;

public sealed class MQTTPackageFactoryPool : IMQTTPackageFactoryPool
{
    private readonly IMQTTPackageFactory[] _packetFactories;
    private readonly Dictionary<Type, IMQTTPackageFactory> _packetFactorieHash = new();

    public MQTTPackageFactoryPool()
    {
        _packetFactories = new IMQTTPackageFactory[20];

        _packetFactories[(byte)MQTTCommand.Connect] = new MQTTPackageFactory<MQTTConnectPackage>();
        _packetFactories[(byte)MQTTCommand.ConnAck] = new MQTTPackageFactory<MQTTConnectRespPackage>();
        _packetFactories[(byte)MQTTCommand.Publish] = new MQTTPackageFactory<MQTTPublishPackage>();
        _packetFactories[(byte)MQTTCommand.PubAck] = new MQTTPackageFactory<MQTTPubRespPackage>();
        _packetFactories[(byte)MQTTCommand.PubComp] = new MQTTPackageFactory<MQTTPubCompPackage>();
        _packetFactories[(byte)MQTTCommand.PubRec] = new MQTTPackageFactory<MQTTPubRecPackage>();
        _packetFactories[(byte)MQTTCommand.SubAck] = new MQTTPackageFactory<MQTTSubRespPackage>();
        _packetFactories[(byte)MQTTCommand.Subscribe] = new MQTTPackageFactory<MQTTSubscribePackage>();
        _packetFactories[(byte)MQTTCommand.Unsubscribe] = new MQTTPackageFactory<MQTTUnsubscribePackage>();
        _packetFactories[(byte)MQTTCommand.UnsubAck] = new MQTTPackageFactory<MQTTUnsubRespPackage>();
        _packetFactories[(byte)MQTTCommand.Ping] = new MQTTPackageFactory<MQTTPingPackage>();
        _packetFactories[(byte)MQTTCommand.PingAck] = new MQTTPackageFactory<MQTTPingRespPackage>();
        _packetFactories[(byte)MQTTCommand.Disconnect] = new MQTTPackageFactory<MQTTDisconnectPackage>();
        _packetFactories[(byte)MQTTCommand.Auth] = new MQTTPackageFactory<MQTTAuthPackage>();

        foreach (var packetFactorie in _packetFactories)
            _packetFactorieHash.Add(packetFactorie.PackageType, packetFactorie);
    }

    public IMQTTPackageFactory Get(MQTTCommand command)
    {
        return _packetFactories[(byte)command];
    }

    public IMQTTPackageFactory Get<TPackage>()
    {
        return _packetFactorieHash[typeof(TPackage)];
    }
}
