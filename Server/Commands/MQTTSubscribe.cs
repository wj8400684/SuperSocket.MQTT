using Core;

namespace Server.Commands;

/// <summary>
/// mqtt 订阅
/// </summary>
[MQTTCommand(MQTTCommand.Subscribe)]
public sealed class MQTTSubscribe : MQTTAsyncCommand<MQTTSubscribePackage, MQTTSubRespPackage>
{
    public MQTTSubscribe(IMQTTPackageFactoryPool packetFactoryPool)
        : base(packetFactoryPool)
    {
    }

    protected override ValueTask<MQTTSubRespPackage?> ExecuteAsync(MQTTSession session, MQTTSubscribePackage package, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
