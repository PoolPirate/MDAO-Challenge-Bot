using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace MDAO_Challenge_Bot.Hangfire;
public class IServiceProviderJobActivatorScope : JobActivatorScope
{
    private readonly IServiceScope Scope;

    public IServiceProviderJobActivatorScope(IServiceScope scope)
    {
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
    }

    public override object Resolve(Type type)
    {
        return ActivatorUtilities.GetServiceOrCreateInstance(Scope.ServiceProvider, type);
    }

    public override void DisposeScope()
    {
        if (Scope is IAsyncDisposable asyncDisposable)
        {
            asyncDisposable.DisposeAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
            return;
        }

        Scope.Dispose();
    }
}
