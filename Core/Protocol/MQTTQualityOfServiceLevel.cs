namespace Core;

public enum MQTTQualityOfServiceLevel
{
    AtMostOnce = 0x00,
    AtLeastOnce = 0x01,
    ExactlyOnce = 0x02
}
