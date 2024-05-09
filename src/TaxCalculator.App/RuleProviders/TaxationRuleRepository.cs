using TaxCalculator.App.Entities;

namespace TaxCalculator.App.RuleProviders;

public interface ITaxationRuleStorage
{
    void Store(IReadOnlyDictionary<int, TaxationRule[]> ruleByYear);
}

public class TaxationRuleRepository : ITaxationRuleStorage, ITaxationRuleProvider
{
    private readonly ITaxationRuleProvider? _decoratee;
    private readonly Dictionary<int, List<TaxationRule>> _storage = new();

    public TaxationRuleRepository(ITaxationRuleProvider? decoratee) => _decoratee = decoratee;

    public void Store(IReadOnlyDictionary<int, TaxationRule[]> rulesByYear)
    {
        foreach (var (year, rules) in rulesByYear)
        {
            if (!_storage.ContainsKey(year)) 
                _storage.Add(year, new List<TaxationRule>());
            
            _storage[year] = _storage[year].Union(rules).ToList();
        }
    }

    public IReadOnlyCollection<TaxationRule> GetRules(int year)
    {
        var decorateeRules = _decoratee?.GetRules(year) ?? Array.Empty<TaxationRule>();

        return _storage.TryGetValue(year, out var rules)
            ? rules.Union(decorateeRules).ToArray()
            : decorateeRules;
    }
}