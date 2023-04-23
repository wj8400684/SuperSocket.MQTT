using System.Runtime.Serialization;

namespace Core;

public sealed class MQTTProtocolViolationException : Exception
{
    public MQTTProtocolViolationException()
    {
    }

    public MQTTProtocolViolationException(string? message) : base(message)
    {
    }

    public MQTTProtocolViolationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}