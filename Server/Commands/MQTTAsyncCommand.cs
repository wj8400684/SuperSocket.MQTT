using Core;
using Microsoft.Extensions.Logging;
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
        var request = (TPackage)package;

        try
        {
            await ExecuteAsync(session, request, cancellationToken);
        }
        catch (Exception e)
        {
            session.LogError(e, $"{session.RemoteAddress}-{package}-{e.Message}");
        }
        finally
        {
            request.Dispose();
        }
    }

    protected abstract ValueTask ExecuteAsync(MQTTSession session, TPackage package, CancellationToken
        cancellationToken);
}

public abstract class MQTTAsyncCommand<TPackage, TRespPackage> : IAsyncCommand<MQTTSession, MQTTPackage>
    where TPackage : MQTTPackage
    where TRespPackage : MQTTPackage
{
    private readonly IMQTTPackageFactory _responseFactory;
    
    public MQTTAsyncCommand(IMQTTPackageFactoryPool packetFactoryPool)
    {
        _responseFactory = packetFactoryPool.Get<TRespPackage>();
    }

    protected TRespPackage CreateResponse() => (TRespPackage)_responseFactory.Create();
    
    ValueTask IAsyncCommand<MQTTSession, MQTTPackage>.ExecuteAsync(MQTTSession session, MQTTPackage package) =>
        SchedulerAsync(session, package, session.ConnectionToken);

    protected virtual async ValueTask SchedulerAsync(MQTTSession session, MQTTPackage package,
        CancellationToken cancellationToken)
    {
        TRespPackage? respPackage = null;

        var request = (TPackage)package;

        try
        {
            respPackage = await ExecuteAsync(session, request, cancellationToken);
        }
        catch (Exception e)
        {
            session.LogError(e, $"{session.RemoteAddress}-{package}-{e.Message}");
        }
        finally
        {
            request.Dispose();
        }

        if (respPackage == null)
            return;

        try
        {
            await session.SendPackageAsync(respPackage);
        }
        finally
        {
            respPackage.Dispose();
        }
    }

    protected abstract ValueTask<TRespPackage?> ExecuteAsync(MQTTSession session, TPackage package, CancellationToken
        cancellationToken);
}