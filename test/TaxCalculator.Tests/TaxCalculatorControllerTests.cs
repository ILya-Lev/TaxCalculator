using System.Globalization;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TaxCalculator.App.Calculators;
using TaxCalculator.App.Controllers;
using TaxCalculator.App.RuleProviders;
using Xunit.Abstractions;

namespace TaxCalculator.Tests;

public class TaxCalculatorControllerTests : IDisposable
{
    private const decimal Income = 500_654.59m;
    private const int Year = 2023;
    
    private readonly ITestOutputHelper _output;
    private readonly IServiceScope _scope;

    private readonly TaxCalculatorController _controller;
    private readonly ICalculator _calculator;

    public TaxCalculatorControllerTests(ITestOutputHelper output)
    {
        _output = output;
        _scope = new ServiceCollection()
            .AddScoped<ICalculator, Calculator>()
            .AddScoped<IRangeCalculator>(_ => new Mock<IRangeCalculator>().Object)//is  not used in a given test set
            .AddScoped<ITaxationRuleProvider, InMemoryTaxationRuleProvider>()
            .Decorate<ITaxationRuleProvider, TaxationRuleRepository>()
            .AddScoped<ITaxationRuleStorage>(_ => new Mock<ITaxationRuleStorage>().Object)//is not used in a given test set
            .AddScoped<TaxCalculatorController>()
            .BuildServiceProvider(validateScopes: true)
            .CreateScope();

        _controller = _scope.ServiceProvider.GetRequiredService<TaxCalculatorController>();
        _calculator = _scope.ServiceProvider.GetRequiredService<ICalculator>();
    }

    public void Dispose() => _scope.Dispose();

    [Fact]
    public async Task Calculate_ViaController_RulesCoverGivenYear_ProduceNumber()
    {
        var result = await _controller.Calculate(Income, Year);

        result.Should().BeOfType<OkObjectResult>();
        
        var value = (result as OkObjectResult)!.Value;
        _output.WriteLine(value?.ToString());
    }

    [Fact]
    public void Calculate_ViaService_RulesCoverGivenYear_ProduceNumber()
    {
        var taxAmount = _calculator.GetTaxAmount(Income, Year);

        taxAmount.Should().BeApproximately(26_532, 1m);
        _output.WriteLine(taxAmount.ToString(CultureInfo.InvariantCulture));
    }

    [Fact]
    public async Task Calculate_ViaController_FutureYear_Error()
    {
        var result = await _controller.Calculate(Income, Year + 10);

        result.Should().BeOfType<BadRequestObjectResult>();
        
        var value = (result as BadRequestObjectResult)!.Value;
        _output.WriteLine(value?.ToString());
    }

    [Fact]
    public void Calculate_ViaService_FutureYear_Error()
    {
        var getTaxAmount = () => _calculator.GetTaxAmount(Income, Year + 10);

        var exc = getTaxAmount.Should().Throw<Exception>().Which;
        _output.WriteLine(exc.Message);
    }
}