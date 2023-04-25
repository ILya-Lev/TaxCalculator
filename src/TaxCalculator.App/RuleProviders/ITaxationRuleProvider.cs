using TaxCalculator.App.Entities;

namespace TaxCalculator.App.RuleProviders;

public interface ITaxationRuleProvider
{
    IReadOnlyCollection<TaxationRule> GetRules(int year);
}