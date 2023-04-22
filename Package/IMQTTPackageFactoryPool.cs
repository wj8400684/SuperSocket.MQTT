namespace Package;

public interface IMQTTPackageFactoryPool
{
    IMQTTPackageFactory Get(MQTTCommand command);
    
    IMQTTPackageFactory Get<TPackage>();
}