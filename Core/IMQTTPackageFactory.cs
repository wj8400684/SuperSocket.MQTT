using Package;

namespace Core;

public interface IMQTTPackageFactory
{
    MQTTPackage Create();

    void Return(MQTTPackage package);
}