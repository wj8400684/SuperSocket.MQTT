using SuperSocket;
using Core;
using Server;
using SuperSocket.Command;
using Server.Commands;
using MQTTServer.Abp;
using SuperSocket.ProtoBase;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AsSuperSocketHostBuilder<MQTTPackage, MQTTPipelineFilter>()
            .UseSession<MQTTSession>()
            .UsePackageDecoder<MQTTPackageDecoder>()
            .UseCommand(options => options.AddCommandAssembly(typeof(MQTTPublish).Assembly))
            .UseClearIdleSession()
            .UseInProcSessionContainer()
            .UseChannelCreatorFactory<TcpIocpChannelWithKestrelCreatorFactory>()
            .AsMinimalApiHostBuilder()
            .ConfigureHostBuilder();

builder.Services.AddSingleton<IMQTTPackageFactoryPool, MQTTPackageFactoryPool>();
builder.Services.AddSingleton<IPackageEncoder<MQTTPackage>, MQTTPackageEncoder>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
