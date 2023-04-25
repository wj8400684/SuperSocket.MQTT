using Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Commands;
using SuperSocket;
using SuperSocket.Command;
using SuperSocket.ProtoBase;

namespace Server;

public static class SuperSocketExtensions
{
    public static IHostBuilder AddMQTTServer(this IHostBuilder hostBuilder)
    {
        hostBuilder.AsSuperSocketHostBuilder<MQTTPackage, MQTTPipelineFilter>()
                   .UsePackageDecoder<MQTTPackageDecoder>()
                   .UseSessionFactory<MQTTSessionFactory>()
                   .UseCommand(options => options.AddCommandAssembly(typeof(MQTTPublish).Assembly))
                   .ConfigureServices((contxt, service) =>
                   {
                       service.AddSingleton<MQTTSubscriberSessionManager>();
                       service.AddSingleton<IMQTTPackageFactoryPool, MQTTPackageFactoryPool>();
                       service.AddSingleton<IPackageEncoder<MQTTPackage>, MQTTPackageEncoder>();
                   })
                   .UseClearIdleSession()
                   .UseInProcSessionContainer()
                   .AsMinimalApiHostBuilder()
                   .ConfigureHostBuilder();

        return hostBuilder;
    }
}
