using TaxCalculator.Seed.Entities;

namespace TaxCalculator.Seed.RuleProviders;

public interface ITaxationRuleProvider
{
    IReadOnlyCollection<TaxationRule> GetRules(int year);
}