using Core;

namespace Server.Commands.Ack;

[MQTTCommand(MQTTCommand.PubAck)]
public sealed class MQTTPublishAck : MQTTAsyncCommand<MQTTPubRespPackage>
{
    protected override ValueTask ExecuteAsync(MQTTSession session, MQTTPubRespPackage package, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}