using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using TaxCalculator.App;
using TaxCalculator.App.Calculators;
using TaxCalculator.App.RuleProviders;

namespace TaxCalculator.InMemoryTests;

public class StaleWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            ReplaceRuleProvider(services);
            //ReplaceCalculator(services);

            RemoveByImplementationTypeName(services, nameof(FileWorker));
        });
        builder.UseEnvironment("Development");
    }

    private void ReplaceRuleProvider(IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Scoped<ITaxationRuleProvider, StaleTaxationRuleProvider>());
    }

    private static void ReplaceCalculator(IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Scoped(typeof(ICalculator), _ =>
        {
            var calculator = new Mock<ICalculator>();
            calculator
                .Setup(c => c.GetTaxAmount(It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(10_000);
            return calculator.Object;
        }));
    }

    private static void RemoveByImplementationTypeName(IServiceCollection services, string name)
    {
        var fileWorkerIndex = services.Select((d, i) => (d, i))
            .Where(item => item.d.ImplementationType is not null)
            .Single(item => item.d.ImplementationType!.Name.Contains(name))
            .i;
        services.RemoveAt(fileWorkerIndex);
    }
}