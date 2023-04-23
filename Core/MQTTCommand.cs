namespace Core;

public enum MQTTCommand
{
    None = 0,
    Connect = 1,
    ConnAck = 2,
    Publish = 3,
    PubAck = 4,
    PubRec = 5,
    PubRel = 6,
    PubComp = 7,
    Subscribe = 8,
    SubAck = 9,
    Unsubscribe = 10,
    UnsubAck = 11,
    Ping = 12,
    PingAck = 13,
    Disconnect = 14,
    Auth = 15
}