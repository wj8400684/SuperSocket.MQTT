using Core;

namespace Server.Commands;

/// <summary>
/// mqtt 连接命令
/// </summary>
[MQTTCommand(MQTTCommand.Connect)]
public sealed class MQTTConnect : MQTTAsyncCommand<MQTTConnectPackage, MQTTConnectRespPackage>
{
    public MQTTConnect(IMQTTPackageFactoryPool packetFactoryPool) : base(packetFactoryPool)
    {
    }

    protected override async ValueTask<MQTTConnectRespPackage> ExecuteAsync(MQTTSession session, MQTTConnectPackage packet, CancellationToken cancellationToken)
    {
        var response = CreateResponse();

        var result = await session.ValidateConnectionaAsync(new ValidatingConnectionResult(packet));

        if (result.ReasonCode != MQTTConnectReasonCode.Success)
        {
            response.ReasonCode = result.ReasonCode;
            return response;
        }

        if (string.IsNullOrWhiteSpace(packet.ClientId))
        {
            session.ClientId = packet.ClientId;
        }
        else
        {
            session.ClientId = packet.ClientId;
        }

        return response;
    }
}