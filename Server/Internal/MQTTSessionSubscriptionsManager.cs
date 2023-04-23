using Core;

namespace Server;

internal sealed class MQTTSessionSubscriptionsManager
{
    private readonly MQTTSession _session;
    private readonly AsyncLock _subscriptionsLock = new();
    private readonly Dictionary<ulong, TopicHashMaskSubscriptions> _wildcardSubscriptionsByTopicHash = new();

    public MQTTSessionSubscriptionsManager(MQTTSession session)
    {
        _session = session;
    }

    public async Task<SubscribeResult> Subscribe(MqttSubscribePacket subscribePacket, CancellationToken cancellationToken)
    {

    }

    public async Task<UnsubscribeResult> Unsubscribe(MqttUnsubscribePacket unsubscribePacket, CancellationToken cancellationToken)
    {
    }

    public CheckSubscriptionsResult CheckSubscriptions(string topic, ulong topicHash, MQTTQualityOfServiceLevel qualityOfServiceLevel, string senderId)
    {

    }

    public void Dispose()
    {
        _subscriptionsLock.Dispose();
        _wildcardSubscriptionsByTopicHash.Clear();
    }
}
