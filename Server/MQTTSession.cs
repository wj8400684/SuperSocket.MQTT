using Core;
using Server.Internal;
using SuperSocket.Channel;
using SuperSocket.ProtoBase;
using SuperSocket.Server;
using System.Net;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Server;

public class MQTTSession : AppSession
{
    private TaskPacketScheduler? _taskScheduler;
    private CancellationTokenSource? _tokenSource;
    private MQTTSubscriptionsManager? _subscriptionsManager;

    private readonly IPackageEncoder<MQTTPackage> _encoder;
    private readonly Dictionary<ushort, string> _topicAlias = new();
    private readonly MQTTSubscriberSessionManager _subscriberSessionManager;

    public MQTTSession(IPackageEncoder<MQTTPackage> encoder, MQTTSubscriberSessionManager subscriberSessionManager)
    {
        _encoder = encoder;
        _subscriberSessionManager = subscriberSessionManager;
    }

    #region property

    public string? ClientId { get; internal set; }

    public string? RemoteAddress { get; private set; }

    public CancellationToken ConnectionToken { get; private set; }

    public DateTime CreatedTimestamp { get; private set; }

    #endregion

    #region protected

    protected override ValueTask OnSessionConnectedAsync()
    {
        _tokenSource = new CancellationTokenSource();
        _taskScheduler = new TaskPacketScheduler(Logger);
        _taskScheduler.Start(_tokenSource.Token);
   
        ConnectionToken = _tokenSource.Token;
        RemoteAddress = ((IPEndPoint)RemoteEndPoint).Address.ToString();

        return ValueTask.CompletedTask;
    }

    protected override void Reset()
    {
        _subscriptionsManager = null;
    }

    protected override ValueTask OnSessionClosedAsync(CloseEventArgs e)
    {
        RemoteAddress = default;
        _topicAlias.Clear();
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
        _subscriptionsManager?.Dispose();

        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask<ValidatingConnectionResult> OnValidateConnectedAsync(
        ValidatingConnectionResult result,
        CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(result);
    }

    protected virtual ValueTask OnClientConnectedAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    #endregion

    #region internal

    internal async ValueTask<ValidatingConnectionResult> ValidateConnectedAsync(ValidatingConnectionResult result,
        CancellationToken cancellationToken)
    {
        CreatedTimestamp = DateTime.UtcNow;
        result.ReasonCode = MQTTConnectReasonCode.Success;

        try
        {
            return await OnValidateConnectedAsync(result, cancellationToken);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "");
        }

        return result;
    }

    internal async ValueTask ClientConnectedAsync(CancellationToken cancellationToken)
    {
        try
        {
            await OnClientConnectedAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "");
        }
    }

    internal void HandleTopicAlias(MQTTPublishPackage publishPackage)
    {
        if (publishPackage.TopicAlias == 0)
            return;

        lock (_topicAlias)
        {
            if (!string.IsNullOrEmpty(publishPackage.Topic))
                _topicAlias[publishPackage.TopicAlias] = publishPackage.Topic;
            else
            {
                if (_topicAlias.TryGetValue(publishPackage.TopicAlias, out var topic))
                    publishPackage.Topic = topic;
                else
                    Logger.LogWarning("Client '{0}': Received invalid topic alias ({1})", ClientId, publishPackage
                    .TopicAlias);
            }
        }
    }

    internal ValueTask<SubscribeResult> SubscribeAsync(MQTTSubscribePackage package, CancellationToken cancellationToken)
    {
        _subscriptionsManager ??= new MQTTSubscriptionsManager(this);
        return _subscriptionsManager.SubscribeAsync(package, cancellationToken);
    }

    internal ValueTask SchedulerAsync(MQTTPackage package,
        Func<MQTTSession, MQTTPackage, CancellationToken, ValueTask> task)
    {
        _taskScheduler?.Add(new TaskItem(this, package, task));

        return ValueTask.CompletedTask;
    }

    internal ValueTask SendPackageAsync(MQTTPackage package)
    {
        return Channel.IsClosed ? ValueTask.CompletedTask : Channel.SendAsync(_encoder, package);
    }

    #endregion
}