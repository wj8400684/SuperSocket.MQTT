using Core;

namespace Server.Commands;

[MQTTCommand(MQTTCommand.PubRec)]
public sealed class MQTTRec : MQTTAsyncCommand<MQTTPubRecPackage>
{
    protected override ValueTask ExecuteAsync(MQTTSession session, MQTTPubRecPackage package, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}