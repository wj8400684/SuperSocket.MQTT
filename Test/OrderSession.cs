using Core;
using Server;
using SuperSocket.ProtoBase;

namespace Test;

public sealed class OrderSession : MQTTSession
{
    public OrderSession(IPackageEncoder<MQTTPackage> encoder) 
        : base(encoder)
    {
    }

    public override ValueTask ClientConnectedAsync()
    {
        return base.ClientConnectedAsync();
    }

    public override ValueTask<ValidatingConnectionResult> ValidateConnectionaAsync(ValidatingConnectionResult result)
    {
        return base.ValidateConnectionaAsync(result);
    }
}
