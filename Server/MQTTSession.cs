using Package;
using SuperSocket.Server;

namespace Server;

public class MQTTSession : AppSession
{
    public async ValueTask SendPackageAsync(MQTTPackage package)
    {
        throw new NotImplementedException();
    }
}