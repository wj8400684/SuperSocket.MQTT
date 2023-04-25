using Core;
using Server;
using SuperSocket.ProtoBase;

namespace MQTTServer.Abp;

internal sealed class MyMQTTSession : MQTTSession
{
    public MyMQTTSession(IPackageEncoder<MQTTPackage> encoder, MQTTSubscriberSessionManager subscriberSessionManager) 
        : base(encoder, subscriberSessionManager)
    {
    }
}
