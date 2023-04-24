using Core;

namespace Server.Commands;

/// <summary>
/// mqtt 心跳
/// </summary>
[MQTTCommand(MQTTCommand.Ping)]
public sealed class MQTTPing : MQTTAsyncCommand<MQTTPingPackage, MQTTPingRespPackage>
{
    public MQTTPing(IMQTTPackageFactoryPool packetFactoryPool)
        : base(packetFactoryPool)
    {
    }

    protected override ValueTask<MQTTPingRespPackage?> ExecuteAsync(MQTTSession session, MQTTPingPackage package, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
