using System.Runtime.Serialization;

namespace Package;

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

    protected MQTTProtocolViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}