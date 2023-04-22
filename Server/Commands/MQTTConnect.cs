using Package;

namespace Server.Commands;

/// <summary>
/// mqtt 连接命令
/// </summary>
[MQTTCommand(MQTTCommand.Connect)]
public sealed class MQTTConnect : MQTTAsyncCommand<MQTTConnectPackage, MQTTConnectRespPackage>
{
    public string RemoteAddress { get; }

    public CancellationToken ConnectionToken { get; }

    protected override ValueTask<MQTTConnectRespPackage> ExecuteAsync(MQTTSession session, MQTTPackage packet,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}