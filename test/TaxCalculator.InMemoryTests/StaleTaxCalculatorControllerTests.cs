using FluentAssertions;
using TaxCalculator.App;

namespace TaxCalculator.InMemoryTests;

[Trait("Category", "Integration")]
public class StaleTaxCalculatorControllerTests : IClassFixture<StaleWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public StaleTaxCalculatorControllerTests(StaleWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Calculate_UsingInfiniteTaxLimit_SomeNumber()
    {
        var raw = await _client
            .GetStringAsync("api/TaxCalculator/Calculate?income=1000000&year=2023");

        var taxAmount = decimal.Parse(raw);

        taxAmount.Should().BeGreaterThanOrEqualTo(10_000M);
    }
}