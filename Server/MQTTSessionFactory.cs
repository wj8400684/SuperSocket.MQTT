using Core;
using SuperSocket;
using SuperSocket.Channel;
using SuperSocket.ProtoBase;
using System.Collections.Concurrent;

namespace Server;

public class MQTTSessionFactory : ISessionFactory
{
    private const int DefaulCount = 100;
    private readonly IPackageEncoder<MQTTPackage> _packageEncoder;
    private readonly MQTTSubscriberSessionManager _subscriberSessionManager;


    private ConcurrentQueue<MQTTSession>? _sessionPool;

    public MQTTSessionFactory(IPackageEncoder<MQTTPackage> packageEncoder, MQTTSubscriberSessionManager subscriberSessionManager)
    {
        _packageEncoder = packageEncoder;
        _subscriberSessionManager = subscriberSessionManager;
        OnIni(packageEncoder);
    }

    public virtual Type SessionType { get; } = typeof(MQTTSession);

    public virtual IAppSession Create()
    {
        if (_sessionPool == null || !_sessionPool.TryDequeue(out var session))
            session = new MQTTSession(_packageEncoder, _subscriberSessionManager);

        session.Closed += OnClosedAsync;

        return session;
    }

    protected virtual void OnIni(IPackageEncoder<MQTTPackage> packageEncoder)
    {
        _sessionPool = new ConcurrentQueue<MQTTSession>();

        for (int i = 0; i < DefaulCount; i++)
            _sessionPool.Enqueue(new MQTTSession(packageEncoder, _subscriberSessionManager));
    }

    protected virtual ValueTask OnClosedAsync(object sender, CloseEventArgs e)
    {
        ((IAppSession)sender).Reset();

        _sessionPool?.Enqueue((MQTTSession)sender);

        return ValueTask.CompletedTask;
    }
}