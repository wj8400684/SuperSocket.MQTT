namespace Core;

public interface IMQTTPackageFactory
{
    Type PackageType { get; }

    MQTTPackage Create();

    void Return(MQTTPackage package);
}