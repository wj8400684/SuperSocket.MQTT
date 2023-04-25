using Core;

namespace Server.Internal;

internal sealed class MQTTSubscriptionsManager
{
    private readonly MQTTSession _session;
    private readonly AsyncLock _subscriptionsLock = new();
    private readonly Dictionary<ulong, TopicHashMaskSubscriptions> _wildcardSubscriptionsByTopicHash = new();

    public MQTTSubscriptionsManager(MQTTSession session)
    {
        _session = session;
    }

    public async ValueTask<SubscribeResult> SubscribeAsync(MQTTSubscribePackage subscribePacket,
        CancellationToken cancellationToken)
    {
        var result = new SubscribeResult(subscribePacket.TopicFilters.Count);

        var addedSubscriptions = new List<string>();
        var finalTopicFilters = new List<MQTTTopicFilter>();

        throw new NotImplementedException();
    }

    public async ValueTask<UnsubscribeResult> UnsubscribeAsync(MQTTUnsubscribePackage unsubscribePacket,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<CheckSubscriptionsResult> CheckSubscriptionsAsync(string topic,
        ulong topicHash,
        MQTTQualityOfServiceLevel qualityOfServiceLevel,
        string senderId,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        // var possibleSubscriptions = new List<MQTTSubscription>();
        //
        // // Check for possible subscriptions. They might have collisions but this is fine.
        // using (var _ = await _subscriptionsLock.EnterAsync(cancellationToken))
        // {
        //     if (_noWildcardSubscriptionsByTopicHash.TryGetValue(topicHash, out var noWildcardSubscriptions))
        //         possibleSubscriptions.AddRange(noWildcardSubscriptions);
        //
        //     foreach (var wcs in _wildcardSubscriptionsByTopicHash)
        //     {
        //         var subscriptionHash = wcs.Key;
        //         var subscriptionsByHashMask = wcs.Value.SubscriptionsByHashMask;
        //         foreach (var shm in subscriptionsByHashMask)
        //         {
        //             var subscriptionHashMask = shm.Key;
        //             if ((topicHash & subscriptionHashMask) == subscriptionHash)
        //             {
        //                 var subscriptions = shm.Value;
        //                 possibleSubscriptions.AddRange(subscriptions);
        //             }
        //         }
        //     }
        // }
        //
        // // The pre check has evaluated that nothing is subscribed.
        // // If there were some possible candidates they get checked below
        // // again to avoid collisions.
        // if (possibleSubscriptions.Count == 0)
        //     return CheckSubscriptionsResult.NotSubscribed;
        //
        // var senderIsReceiver = string.Equals(senderId, _session.ClientId);
        // var maxQoSLevel = -1; // Not subscribed.
        //
        // HashSet<uint>? subscriptionIdentifiers = null;
        // var retainAsPublished = false;
        //
        // foreach (var subscription in possibleSubscriptions)
        // {
        //     if (subscription.NoLocal && senderIsReceiver)// This is a MQTTv5 feature!
        //         continue;
        //
        //     if (MQTTTopicFilterComparer.Compare(topic, subscription.Topic) != MQTTTopicFilterCompareResult.IsMatch)
        //         continue;
        //
        //     if (subscription.RetainAsPublished)// This is a MQTTv5 feature!
        //         retainAsPublished = true;
        //
        //     if ((int)subscription.GrantedQualityOfServiceLevel > maxQoSLevel)
        //         maxQoSLevel = (int)subscription.GrantedQualityOfServiceLevel;
        //
        //     if (subscription.Identifier > 0)
        //     {
        //         subscriptionIdentifiers ??= new HashSet<uint>();
        //
        //         subscriptionIdentifiers.Add(subscription.Identifier);
        //     }
        // }
        //
        // if (maxQoSLevel == -1)
        //     return CheckSubscriptionsResult.NotSubscribed;
        //
        // var result = new CheckSubscriptionsResult
        // {
        //     IsSubscribed = true,
        //     RetainAsPublished = retainAsPublished,
        //     SubscriptionIdentifiers = subscriptionIdentifiers?.ToList() ?? new(),
        //
        //     // Start with the same QoS as the publisher.
        //     QualityOfServiceLevel = qualityOfServiceLevel
        // };
        //
        // // Now downgrade if required.
        // //
        // // If a subscribing Client has been granted maximum QoS 1 for a particular Topic Filter, then a QoS 0 Application Message matching the filter is delivered
        // // to the Client at QoS 0. This means that at most one copy of the message is received by the Client. On the other hand, a QoS 2 Message published to
        // // the same topic is downgraded by the Server to QoS 1 for delivery to the Client, so that Client might receive duplicate copies of the Message.
        //
        // // Subscribing to a Topic Filter at QoS 2 is equivalent to saying "I would like to receive Messages matching this filter at the QoS with which they were published".
        // // This means a publisher is responsible for determining the maximum QoS a Message can be delivered at, but a subscriber is able to require that the Server
        // // downgrades the QoS to one more suitable for its usage.
        // if (maxQoSLevel < (int)qualityOfServiceLevel)
        //     result.QualityOfServiceLevel = (MQTTQualityOfServiceLevel)maxQoSLevel;
        //
        // return result;
    }

    public void Dispose()
    {
        _subscriptionsLock.Dispose();
        _wildcardSubscriptionsByTopicHash.Clear();
    }

    sealed class CreateSubscriptionResult
    {
        public required bool IsNewSubscription { get; set; }

        public required MQTTSubscription Subscription { get; set; }
    }
}