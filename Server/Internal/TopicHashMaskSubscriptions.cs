namespace Server;

internal sealed  class TopicHashMaskSubscriptions
{
    public Dictionary<ulong, HashSet<MQTTSubscription>> SubscriptionsByHashMask { get; } = new Dictionary<ulong, HashSet<MQTTSubscription>>();

    public void AddSubscription(MQTTSubscription subscription)
    {
        if (!SubscriptionsByHashMask.TryGetValue(subscription.TopicHashMask, out var subscriptions))
        {
            subscriptions = new HashSet<MQTTSubscription>();
            SubscriptionsByHashMask.Add(subscription.TopicHashMask, subscriptions);
        }
        subscriptions.Add(subscription);
    }

    public void RemoveSubscription(MQTTSubscription subscription)
    {
        if (SubscriptionsByHashMask.TryGetValue(subscription.TopicHashMask, out var subscriptions))
        {
            subscriptions.Remove(subscription);
            if (subscriptions.Count == 0)
            {
                SubscriptionsByHashMask.Remove(subscription.TopicHashMask);
            }
        }
    }
}
