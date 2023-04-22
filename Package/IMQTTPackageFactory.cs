namespace Package;

public interface IMQTTPackageFactory
{
    MQTTPackage Create();

    void Return(MQTTPackage package);
}