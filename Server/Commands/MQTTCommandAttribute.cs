using Core;
using SuperSocket.Command;

namespace Server.Commands;

public sealed class MQTTCommandAttribute : CommandAttribute
{
    public MQTTCommand Command { get; }

    public MQTTCommandAttribute(MQTTCommand key)
    {
        Key = (int)key;
        Command = key;
    }
}