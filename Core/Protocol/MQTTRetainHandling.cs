namespace Core;

public enum MQTTRetainHandling
{
    SendAtSubscribe = 0,

    SendAtSubscribeIfNewSubscriptionOnly = 1,

    DoNotSendOnSubscribe = 2
}
