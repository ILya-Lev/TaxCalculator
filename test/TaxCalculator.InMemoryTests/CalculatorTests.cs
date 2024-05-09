using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TaxCalculator.App;
using TaxCalculator.App.Calculators;
using Xunit.Abstractions;

namespace TaxCalculator.InMemoryTests;

[Trait("Category", "Integration")]
public class CalculatorTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private const decimal Income = 1_765_791.03m;
    private const int Year = 2022;
    
    private readonly ITestOutputHelper _output;

    private readonly IServiceScope _scope;
    private readonly ICalculator _calculator;

    public CalculatorTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _output = output;
        _scope = factory.Services.CreateScope();
        _calculator = _scope.ServiceProvider.GetRequiredService<ICalculator>();
    }

    public void Dispose() => _scope.Dispose();

    [Fact]
    public void GetTaxAmount_CoveredYear_ProduceNumber()
    {
        _calculator.GetTaxAmount(Income, Year).Should().BeApproximately(159_921, 1m);
    }

    [Fact]
    public void GetTaxAmount_UncoveredYear_Exception()
    {
        var getTaxAmount = () => _calculator.GetTaxAmount(Income, Year+20);
            
        var exc = getTaxAmount.Should().Throw<Exception>().Which;
        _output.WriteLine(exc.Message);
    }
}