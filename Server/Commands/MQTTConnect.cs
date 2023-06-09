using Core;
using SuperSocket;

namespace Server.Commands;

/// <summary>
/// mqtt 连接命令
/// </summary>
[MQTTCommand(MQTTCommand.Connect)]
internal sealed class MQTTConnect : MQTTAsyncCommand<MQTTConnectPackage, MQTTConnectRespPackage>
{
    public MQTTConnect(IMQTTPackageFactoryPool packetFactoryPool)
        : base(packetFactoryPool)
    {
    }

    protected override async ValueTask<MQTTConnectRespPackage?> ExecuteAsync(MQTTSession session,
        MQTTConnectPackage packet, CancellationToken cancellationToken)
    {
        var response = CreateResponse();

        var result =
            await session.ValidateConnectedAsync(new ValidatingConnectionResult(packet, response), cancellationToken);

        if (result.ReasonCode != MQTTConnectReasonCode.Success)
            return result.GetResponse();

        session.ClientId = string.IsNullOrWhiteSpace(packet.ClientId)
            ? result.AssignedClientIdentifier
            : packet.ClientId;

        Task.Run(() => session.ClientConnectedAsync(cancellationToken)).DoNotAwait();

        return result.GetResponse();
    }
}