using Core;

namespace Server;

internal sealed class MQTTSubscription
{
    public MQTTSubscription(
        string topic,
        bool noLocal,
        MqttRetainHandling retainHandling,
        bool retainAsPublished,
        MqttQualityOfServiceLevel qualityOfServiceLevel,
        uint identifier)
    {
        Topic = topic;
        NoLocal = noLocal;
        RetainHandling = retainHandling;
        RetainAsPublished = retainAsPublished;
        GrantedQualityOfServiceLevel = qualityOfServiceLevel;
        Identifier = identifier;

        MqttTopicHash.Calculate(Topic, out var hash, out var hashMask, out var hasWildcard);
        TopicHash = hash;
        TopicHashMask = hashMask;
        TopicHasWildcard = hasWildcard;
    }

    public MQTTQualityOfServiceLevel GrantedQualityOfServiceLevel { get; }

    public uint Identifier { get; }

    public bool NoLocal { get; }

    public bool RetainAsPublished { get; }

    public MQTTRetainHandling RetainHandling { get; }

    public string Topic { get; }

    public ulong TopicHash { get; }

    public ulong TopicHashMask { get; }

    public bool TopicHasWildcard { get; }
}
