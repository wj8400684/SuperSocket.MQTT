using Microsoft.Extensions.Logging;
using SuperSocket;

namespace Server;

/// <summary>
/// 所有消费者集合
/// </summary>
public sealed class MQTTSubscriberSessionManager
{
    private readonly AsyncLock _sync = new();
    private readonly IAsyncSessionContainer _sessionContainer;
    private readonly ILogger<MQTTSubscriberSessionManager> _logger;
    private readonly HashSet<MQTTSession> _subscriberSessions = new();

    public MQTTSubscriberSessionManager(ILogger<MQTTSubscriberSessionManager> logger, IAsyncSessionContainer sessionContainer)
    {
        _logger = logger;
        _sessionContainer = sessionContainer;
    }

    internal ValueTask DispatchApplicationMessageAsync(string clientId, CancellationToken cancellationToken)
    {

    }
}
