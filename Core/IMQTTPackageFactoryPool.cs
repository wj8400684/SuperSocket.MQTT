namespace Core;

public interface IMQTTPackageFactoryPool
{
    IMQTTPackageFactory Get(MQTTCommand command);

    IMQTTPackageFactory Get<TPackage>();
}