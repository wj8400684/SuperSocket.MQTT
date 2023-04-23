using Server;

namespace Test;

public sealed class OrderSession : MQTTSession
{
    public OrderSession(IServiceProvider serviceProvider) 
        : base(serviceProvider)
    {
    }

    public override ValueTask ClientConnectedAsync(CancellationToken cancellationToken)
    {
        return base.ClientConnectedAsync(cancellationToken);
    }

    public override ValueTask<ValidatingConnectionResult> ValidateConnectionaAsync(ValidatingConnectionResult result, CancellationToken cancellationToken)
    {
        return base.ValidateConnectionaAsync(result, cancellationToken);
    }
}
