using Core;

namespace Server.Commands;

/// <summary>
/// mqtt 取消订阅
/// </summary>
[MQTTCommand(MQTTCommand.Unsubscribe)]
public sealed class MQTTUnsubscribe : MQTTAsyncCommand<MQTTUnsubscribePackage, MQTTUnsubRespPackage>
{
    public MQTTUnsubscribe(IMQTTPackageFactoryPool packetFactoryPool)
        : base(packetFactoryPool)
    {
    }

    protected override ValueTask<MQTTUnsubRespPackage?> ExecuteAsync(MQTTSession session,
        MQTTUnsubscribePackage package, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}