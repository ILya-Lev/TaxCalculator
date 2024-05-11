using TaxCalculator.App.Entities;
using TaxCalculator.App.RuleProviders;

namespace TaxCalculator.InMemoryTests;

internal class StaleTaxationRuleProvider : ITaxationRuleProvider, ITaxationRuleStorage
{
    public IReadOnlyCollection<TaxationRule> GetRules(int year)
        => [new TaxationRule(100_000_000, 10_000, 3)];

    public void Store(IReadOnlyDictionary<int, TaxationRule[]> ruleByYear) => throw new NotImplementedException();
}