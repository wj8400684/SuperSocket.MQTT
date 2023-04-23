using Core;

namespace Server;

internal sealed class SubscribeResult
{
    public SubscribeResult(int topicsCount)
    {
        ReasonCodes = new List<MQTTSubscribeReasonCode>(topicsCount);
    }

    public bool CloseConnection { get; set; }

    public List<MQTTSubscribeReasonCode> ReasonCodes { get; set; }

    public string? ReasonString { get; set; }

    public List<MQTTRetainedMessageMatch>? RetainedMessages { get; set; }

    public List<MQTTUserProperty>? UserProperties { get; set; }
}
