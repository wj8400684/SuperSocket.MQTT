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

    #region property

    /// <summary>
    /// 客户端id
    /// </summary>
    public string? ClientId { get; internal set; }

    /// <summary>
    /// 远程地址
    /// </summary>
    public string RemoteAddress { get; private set; } = default!;

    /// <summary>
    /// 连接token
    /// </summary>
    public CancellationToken ConnectionToken { get; private set; }

    /// <summary>
    /// 客户端连接时间
    /// </summary>
    public DateTime CreatedTimestamp { get; private set; }

    #endregion

    #region protected

    protected override ValueTask OnSessionConnectedAsync()
    {
        RemoteAddress = ((IPEndPoint)RemoteEndPoint).Address.ToString();

        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnSessionClosedAsync(CloseEventArgs e)
    {
        _tokenSource.Cancel();
        _tokenSource.Dispose();

        return base.OnSessionClosedAsync(e);
    }

    #endregion

    #region public

    /// <summary>
    /// 验证连接
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public virtual ValueTask<ValidatingConnectionResult> ValidateConnectionaAsync(ValidatingConnectionResult result)
    {
        CreatedTimestamp = DateTime.UtcNow;
        return ValueTask.FromResult(result);
    }

    /// <summary>
    /// 客户端连接成功
    /// </summary>
    /// <returns></returns>
    public virtual ValueTask ClientConnectedAsync()
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 发送包
    /// </summary>
    /// <param name="package"></param>
    /// <returns></returns>
    public ValueTask SendPackageAsync(MQTTPackage package)
    {
        if (Channel.IsClosed)
            return ValueTask.CompletedTask;

        return Channel.SendAsync(_encoder, package);
    }

    #endregion
}