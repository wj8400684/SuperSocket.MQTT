using Core;
using SuperSocket.Channel;
using SuperSocket.ProtoBase;
using SuperSocket.Server;
using System.Net;

namespace Server;

public class MQTTSession : AppSession
{
    private readonly IPackageEncoder<MQTTPackage> _encoder;
    private readonly CancellationTokenSource _tokenSource = new();

    public MQTTSession(IPackageEncoder<MQTTPackage> encoder)
    {
        _encoder = encoder;
        ConnectionToken = _tokenSource.Token;
    }

    public string? ClientId { get; set; }

    public string RemoteAddress { get; private set; } = default!;

    public CancellationToken ConnectionToken { get; private set; }

    protected override ValueTask OnSessionConnectedAsync()
    {
        RemoteAddress = ((IPEndPoint)RemoteEndPoint).Address.ToString();

        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnSessionClosedAsync(CloseEventArgs e)
    {
        return base.OnSessionClosedAsync(e);
    }

    public ValueTask<bool> ValidateConnectionaAsync(MQTTConnectPackage package)
    {
        return ValueTask.FromResult(false);
    }

    public ValueTask SendPackageAsync(MQTTPackage package)
    {
        if (Channel.IsClosed)
            return ValueTask.CompletedTask;

        return Channel.SendAsync(_encoder, package);
    }
}