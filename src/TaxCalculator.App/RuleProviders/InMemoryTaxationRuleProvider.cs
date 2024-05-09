using TaxCalculator.App.Entities;

namespace TaxCalculator.App.RuleProviders;

public class InMemoryTaxationRuleProvider : ITaxationRuleProvider
{
    private static readonly Dictionary<int, IReadOnlyCollection<TaxationRule>> _map = new()
    {
        [2022] = new TaxationRule[]
        {
            new(500_000, 300, 5),
            new(1_000_000, 500, 7),
            new(5_000_000, 1_000, 9),
            new(5_500_000, 1_500, 11),
        },
        [2023] = new TaxationRule[]
        {
            new(1_500_000, 1_500, 5),
            new(3_000_000, 3_500, 7),
            new(7_000_000, 7_500, 9),
            new(9_500_000, 9_800, 11),
        },
    };

    public IReadOnlyCollection<TaxationRule> GetRules(int year) => _map
        .TryGetValue(year, out var rules) 
        ? rules 
        : Array.Empty<TaxationRule>();
}