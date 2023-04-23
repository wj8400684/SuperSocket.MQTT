namespace Core;


public sealed class MQTTCommunicationException : Exception
{
    public MQTTCommunicationException()
    {
    }

    public MQTTCommunicationException(string? message) : base(message)
    {
    }

    public MQTTCommunicationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
