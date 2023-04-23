using Core;

namespace Server;

internal sealed class CheckSubscriptionsResult
{
    public static CheckSubscriptionsResult NotSubscribed { get; } = new CheckSubscriptionsResult();

    public bool IsSubscribed { get; set; }

    public bool RetainAsPublished { get; set; }

    public List<uint>? SubscriptionIdentifiers { get; set; }

    public MQTTQualityOfServiceLevel QualityOfServiceLevel { get; set; }
}
