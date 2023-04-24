using Core;
using Server.Internal;
using SuperSocket.Channel;
using SuperSocket.ProtoBase;
using SuperSocket.Server;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Server;

public class MQTTSession : AppSession
{
    private readonly IPackageEncoder<MQTTPackage> _encoder;
    private readonly TaskPacketScheduler _taskScheduler;
    private readonly CancellationTokenSource _tokenSource = new();
    private readonly MQTTSubscriptionsManager _subscriptionsManager;
    private readonly Dictionary<ushort, string> _topicAlias = new();

    public MQTTSession(IPackageEncoder<MQTTPackage> encoder)
    {
        _encoder = encoder;
        _taskScheduler = new TaskPacketScheduler(Logger);
        _subscriptionsManager = new MQTTSubscriptionsManager(this);
        ConnectionToken = _tokenSource.Token;
    }

    #region property

    public string? ClientId { get; internal set; }

    public string RemoteAddress { get; private set; } = default!;

    public CancellationToken ConnectionToken { get; private set; }

    public DateTime CreatedTimestamp { get; private set; }

    #endregion

    #region protected

    protected override ValueTask OnSessionConnectedAsync()
    {
        _taskScheduler.Start(_tokenSource.Token);
        RemoteAddress = ((IPEndPoint)RemoteEndPoint).Address.ToString();

        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnSessionClosedAsync(CloseEventArgs e)
    {
        _tokenSource.Cancel();
        _tokenSource.Dispose();
        _subscriptionsManager.Dispose();

        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask<ValidatingConnectionResult> OnValidateConnectedAsync(
        ValidatingConnectionResult result,
        CancellationToken cancellationToken)
    {
        result.ReasonCode = MQTTConnectReasonCode.Success;
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
        try
        {
            CreatedTimestamp = DateTime.UtcNow;
            return await OnValidateConnectedAsync(result, cancellationToken);
        }
        catch (Exception e)
        {
            this.Logger.LogError(e, "");
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
            this.Logger.LogError(e, "");
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

    internal ValueTask SchedulerAsync(MQTTPackage package,
        Func<MQTTSession, MQTTPackage, CancellationToken, ValueTask> task)
    {
        _taskScheduler.Add(new TaskItem(this, package, task));

        return ValueTask.CompletedTask;
    }

    internal ValueTask SendPackageAsync(MQTTPackage package)
    {
        return Channel.IsClosed ? ValueTask.CompletedTask : Channel.SendAsync(_encoder, package);
    }

    #endregion
}