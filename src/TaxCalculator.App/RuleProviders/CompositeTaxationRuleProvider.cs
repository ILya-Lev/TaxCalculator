using TaxCalculator.App.Entities;

namespace TaxCalculator.App.RuleProviders;

public class CompositeTaxationRuleProvider : ITaxationRuleProvider
{
    private readonly IReadOnlyCollection<ITaxationRuleProvider> _providers;

    public CompositeTaxationRuleProvider(IEnumerable<ITaxationRuleProvider> providers)
    {
        _providers = providers?.Distinct().ToArray() ?? Array.Empty<ITaxationRuleProvider>();
        if (!_providers.Any()) 
            throw new ArgumentException($"No {nameof(ITaxationRuleProvider)} is passed");
    }

    public IReadOnlyCollection<TaxationRule> GetRules(int year) => _providers.Aggregate(
        seed: Enumerable.Empty<TaxationRule>()
        , func: (acc, p) => acc.Union(p.GetRules(year))
        , resultSelector: r => r.Distinct().ToArray());
}