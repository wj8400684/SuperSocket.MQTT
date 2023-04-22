using Microsoft.Extensions.Logging;
using Package;
using SuperSocket.Command;

namespace Server.Commands;

public abstract class MQTTAsyncCommand<TPackage> : IAsyncCommand<MQTTSession, MQTTPackage>
    where TPackage : MQTTPackage
{
    ValueTask IAsyncCommand<MQTTSession, MQTTPackage>.ExecuteAsync(MQTTSession session, MQTTPackage package) =>
        SchedulerAsync(session, package, session.ConnectionToken);

    protected virtual async ValueTask SchedulerAsync(MQTTSession session, MQTTPackage package,
        CancellationToken cancellationToken)
    {
        var request = (MQTTPackage)package;

        try
        {
            await ExecuteAsync(session, request, cancellationToken);
        }
        catch (Exception e)
        {
            //session.LogError(e, $"{session.RemoteAddress}-{package.Key} 抛出一个未知异常");
        }
        finally
        {
            package.Dispose();
        }
    }

    protected abstract ValueTask ExecuteAsync(MQTTSession session, MQTTPackage packet, CancellationToken
        cancellationToken);
}

public abstract class MQTTAsyncCommand<TPackage, TRespPackage> : IAsyncCommand<MQTTSession, MQTTPackage>
    where TPackage : MQTTPackage
    where TRespPackage : MQTTPackage
{
    private readonly IMQTTPackageFactory _packetFactory;

    public MQTTAsyncCommand(IMQTTPackageFactoryPool packetFactoryPool)
    {
        _packetFactory = packetFactoryPool.Get<TRespPackage>();
    }

    ValueTask IAsyncCommand<MQTTSession, MQTTPackage>.ExecuteAsync(MQTTSession session, MQTTPackage package) =>
        SchedulerAsync(session, package, session.ConnectionToken);

    protected virtual async ValueTask SchedulerAsync(MQTTSession session, MQTTPackage package,
        CancellationToken cancellationToken)
    {
        TRespPackage respPackage;
        var request = (MQTTPackage)package;

        try
        {
            respPackage = await ExecuteAsync(session, request, cancellationToken);
        }
        catch (Exception e)
        {
            //session.LogError(e, $"{session.RemoteAddress}-{package.Key} 抛出一个未知异常");
        }
        finally
        {
            package.Dispose();
        }

        try
        {
            await session.SendPackageAsync(respPackage);
        }
        finally
        {
            respPackage.Dispose();
        }
    }

    protected abstract ValueTask<TRespPackage> ExecuteAsync(MQTTSession session, MQTTPackage packet, CancellationToken
        cancellationToken);
}