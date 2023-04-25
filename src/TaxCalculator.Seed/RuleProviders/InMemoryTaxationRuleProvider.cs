using TaxCalculator.Seed.Entities;

namespace TaxCalculator.Seed.RuleProviders;

public class InMemoryTaxationRuleProvider : ITaxationRuleProvider
{
    private static readonly Dictionary<int, IReadOnlyCollection<TaxationRule>> _map = new()
    {
        [2023] = new TaxationRule[]
        {
            new(1_000_000, 500, 3),
            new(3_000_000, 1_000, 4),
            new(7_000_000, 1_500, 5),
            new(9_000_000, 2_500, 7),
        },
        [2022] = new TaxationRule[]
        {
            new(500_000, 300, 5),
            new(1_000_000, 500, 7),
            new(5_000_000, 1_000, 9),
            new(5_500_000, 1_500, 11),
        },
    };

    public IReadOnlyCollection<TaxationRule> GetRules(int year) => _map
        .TryGetValue(year, out var rules) 
        ? rules 
        : Array.Empty<TaxationRule>();
}