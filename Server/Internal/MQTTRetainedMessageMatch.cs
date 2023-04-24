using Core;

namespace Server;

internal sealed record MQTTApplicationMessage();

internal sealed record MQTTRetainedMessageMatch(MQTTApplicationMessage ApplicationMessage, MQTTQualityOfServiceLevel SubscriptionQualityOfServiceLevel);
