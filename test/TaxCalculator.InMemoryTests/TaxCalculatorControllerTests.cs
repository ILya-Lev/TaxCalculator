using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using TaxCalculator.App;
using TaxCalculator.App.Entities;
using Xunit.Abstractions;

namespace TaxCalculator.InMemoryTests;

[Trait("Category", "Integration")]
public class TaxCalculatorControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly ITestOutputHelper _output;
    private readonly HttpClient _client;

    public TaxCalculatorControllerTests(
        WebApplicationFactory<Program> factory
        , ITestOutputHelper output)
    {
        _output = output;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Calculate_StandardFlow_Ok()
    {
        var raw = await _client.GetStringAsync("api/TaxCalculator/Calculate" +
                                               "?income=1000000&year=2023");
        
        decimal.TryParse(raw, out var amount).Should().BeTrue();
        amount.Should().Be(51_500);
    }

    [Fact]
    public async Task Calculate_AbsentTaxationRule_BadRequest()
    {
        var response = await _client.GetAsync("api/TaxCalculator/Calculate" +
                                               "?income=1000000&year=2020");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var message = await response.Content.ReadAsStringAsync();
        _output.WriteLine(message);
    }

    [Fact]
    public async Task CalculateForRange_StandardFlow_Ok()
    {
        var calculationModel = new RangeCalculationModel()
        {
            IncomeByYear = new Dictionary<int, decimal>()
            {
                [2021] = 1_002_983,
                [2022] = 1_500_000,
                [2023] = 2_300_671,
            },
            RulesByYear = new Dictionary<int, TaxationRule[]>()
            {
                [2021] = new TaxationRule[]
                {
                    new(2_000_000, 1_000,  4)
                }
            }
        };

        var response = await _client
            .PostAsJsonAsync("api/TaxCalculator/CalculateForRange", calculationModel);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var raw = await response.Content.ReadAsStringAsync();
        var taxesByYear = JsonSerializer.Deserialize<Dictionary<int, decimal?>>(raw);

        using var scope = new AssertionScope();
        taxesByYear.Should().NotBeNull();
        taxesByYear![2021].Should().BeApproximately(41_119, 1);
        taxesByYear[2022].Should().Be(136_000);
        taxesByYear[2023].Should().BeApproximately(164_546, 1);
        _output.WriteLine(raw);
    }
}
/*
 * sample request for manual testing
{
  "incomeByYear": {"2021": 1500000,"2022":1000000,"2023":2000000},
  "rulesByYear": {
    "2021": [
        { "upperBound": 1000000, "rate": 3, "fixedPayment": 750 },
        { "upperBound": 2000000, "rate": 4, "fixedPayment": 1500 }
    ]
  }
}
 *
 */