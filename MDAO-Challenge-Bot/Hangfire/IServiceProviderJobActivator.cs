using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace MDAO_Challenge_Bot.Hangfire;
public class IServiceProviderJobActivator : JobActivator
{
    private readonly IServiceScopeFactory ServiceScopeFactory;

    public IServiceProviderJobActivator(IServiceScopeFactory serviceScopeFactory)
    {
        ServiceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    public override JobActivatorScope BeginScope(JobActivatorContext context)
    {
        return new IServiceProviderJobActivatorScope(ServiceScopeFactory.CreateScope());
    }
}
