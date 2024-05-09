using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using TaxCalculator.App.Calculators;
using TaxCalculator.App.Entities;
using TaxCalculator.App.RuleProviders;

namespace TaxCalculator.InMemoryTests;

public class StaleWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.Replace(ServiceDescriptor.Scoped(typeof(ICalculator), _ =>
            {
                var calculator = new Mock<ICalculator>();
                calculator
                    .Setup(c => c.GetTaxAmount(It.IsAny<decimal>(), It.IsAny<int>()))
                    .Returns(10_000);
                return calculator.Object;
            }));
            
            //ReplaceRuleProvider(services);
        });
        builder.UseEnvironment("Development");
    }

    private static void ReplaceRuleProvider(IServiceCollection services)
    {
        //implementation of lib Replace(), but with ServiceType.Name.Contains() instead of ServiceType ==
        for (int i = services.Count - 1; i >= 0; i--)
        {
            ServiceDescriptor? descriptor = services[i];
            if (descriptor.ServiceType.Name.Contains(nameof(ITaxationRuleProvider)))
                services.RemoveAt(i);
        }

        services.AddScoped<ITaxationRuleProvider, StaleTaxationRuleProvider>();
    }
}

internal class StaleTaxationRuleProvider : ITaxationRuleProvider
{
    public IReadOnlyCollection<TaxationRule> GetRules(int year)
        => [new TaxationRule(100_000_000, 10_000, 3)];
}