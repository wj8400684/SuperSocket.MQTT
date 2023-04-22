using Package;
using SuperSocket.Server;

namespace Server;

public class MQTTSession : AppSession
{
    public async ValueTask SendPacketAsync(MQTTPackage package)
    {
        throw new NotImplementedException();
    }
}