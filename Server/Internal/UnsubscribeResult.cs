using Core;

namespace Server;

internal sealed class UnsubscribeResult
{
    public List<MQTTUnsubscribeReasonCode> ReasonCodes { get; } = new List<MQTTUnsubscribeReasonCode>(128);
    public bool CloseConnection { get; set; }

    public List<MQTTUserProperty>? UserProperties { get; set; }
}
