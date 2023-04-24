using Core;
using Server;
using SuperSocket;
using SuperSocket.ProtoBase;

namespace MQTTServer.Abp;

/// <summary>
/// 直接创建 mqtt session 相比用范型创建性能高一些
/// </summary>
internal sealed class MQTTSessionFactory : ISessionFactory
{
    private readonly IPackageEncoder<MQTTPackage> _packageEncoder;
    
    public MQTTSessionFactory(IPackageEncoder<MQTTPackage> packageEncoder)
    {
        _packageEncoder = packageEncoder;
    }
    
    public IAppSession Create()
    {
        return new MQTTSession(_packageEncoder);
    }

    public Type SessionType { get; } = typeof(MQTTSession);
}