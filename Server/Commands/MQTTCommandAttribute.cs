using Package;
using SuperSocket.Command;

namespace Server.Commands;

public sealed class MQTTCommandAttribute : CommandAttribute
{
    public MQTTCommand Command { get; }

    public MQTTCommandAttribute(MQTTCommand key)
    {
        Key = (byte)key;
        Command = key;
    }
}