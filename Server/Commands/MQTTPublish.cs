using Core;

namespace Server.Commands;

/// <summary>
/// mqtt 推送命令
/// </summary>
[MQTTCommand(MQTTCommand.Publish)]
public sealed class MQTTPublish : MQTTAsyncCommand<MQTTPublishPackage, MQTTPubRecPackage>
{
    public MQTTPublish(IMQTTPackageFactoryPool packetFactoryPool)
        : base(packetFactoryPool)
    {
    }

    protected override ValueTask SchedulerAsync(MQTTSession session, MQTTPackage package, CancellationToken cancellationToken) => session.SchedulerAsync(package, base.SchedulerAsync);

    protected override async ValueTask<MQTTPubRecPackage?> ExecuteAsync(MQTTSession session, MQTTPublishPackage package, CancellationToken cancellationToken)
    {
        session.HandleTopicAlias(package);

        MQTTPubRecPackage? response = null;

        switch (package.QualityOfServiceLevel)
        {
            case MQTTQualityOfServiceLevel.AtMostOnce:
                // Do nothing since QoS 0 has no ACK at all!
                return null;
            case MQTTQualityOfServiceLevel.AtLeastOnce:
                break;
            case MQTTQualityOfServiceLevel.ExactlyOnce:
                break;
            default:
                throw new MQTTCommunicationException("Received a not supported QoS level");
        }

        return response;
    }
}
