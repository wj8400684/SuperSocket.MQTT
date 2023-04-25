using Core;

namespace Server;

internal sealed class MQTTPackageFactoryPool : IMQTTPackageFactoryPool
{
    private readonly IMQTTPackageFactory[] _packetFactories = new IMQTTPackageFactory[20];
    private readonly Dictionary<Type, IMQTTPackageFactory> _packetFactorieHash = new();

    public MQTTPackageFactoryPool()
    {
        RegisterPacketType<MQTTConnectPackage>(MQTTCommand.Connect);
        RegisterPacketType<MQTTConnectRespPackage>(MQTTCommand.ConnAck);
        RegisterPacketType<MQTTPublishPackage>(MQTTCommand.Publish);
        RegisterPacketType<MQTTPubRespPackage>(MQTTCommand.PubAck);
        RegisterPacketType<MQTTPubCompPackage>(MQTTCommand.PubComp);
        RegisterPacketType<MQTTPubRecPackage>(MQTTCommand.PubRec);
        RegisterPacketType<MQTTPubRelPackage>(MQTTCommand.PubRel);
        RegisterPacketType<MQTTSubRespPackage>(MQTTCommand.SubAck);
        RegisterPacketType<MQTTSubscribePackage>(MQTTCommand.Subscribe);
        RegisterPacketType<MQTTUnsubscribePackage>(MQTTCommand.Unsubscribe);
        RegisterPacketType<MQTTUnsubRespPackage>(MQTTCommand.UnsubAck);
        RegisterPacketType<MQTTPingPackage>(MQTTCommand.Ping);
        RegisterPacketType<MQTTPingRespPackage>(MQTTCommand.PingAck);
        RegisterPacketType<MQTTDisconnectPackage>(MQTTCommand.Disconnect);
        RegisterPacketType<MQTTAuthPackage>(MQTTCommand.Auth);

        foreach (var packetFactorie in _packetFactories)
        {
            if (packetFactorie != null)
                _packetFactorieHash.Add(packetFactorie.PackageType, packetFactorie);
        }
    }

    public IMQTTPackageFactory Get(MQTTCommand command)
    {
        return _packetFactories[(byte)command];
    }

    public IMQTTPackageFactory Get<TPackage>()
    {
        return _packetFactorieHash[typeof(TPackage)];
    }

    private void RegisterPacketType<TPackage>(MQTTCommand command)
        where TPackage : MQTTPackage, new()
    {
        _packetFactories[(byte)command] = new MQTTPackageFactory<TPackage>();
    }
}