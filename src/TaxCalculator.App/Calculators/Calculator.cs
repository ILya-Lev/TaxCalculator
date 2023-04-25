using TaxCalculator.App.RuleProviders;

namespace TaxCalculator.App.Calculators;

public interface ICalculator
{
    decimal GetTaxAmount(decimal income, int year);
}

public class Calculator : ICalculator
{
    private readonly ITaxationRuleProvider _rulesProvider;
    public Calculator(ITaxationRuleProvider rulesProvider) => _rulesProvider = rulesProvider;

    public decimal GetTaxAmount(decimal income, int year)
    {
        var rules = _rulesProvider.GetRules(year);
        
        var applicableRule = rules
            .OrderBy(r => r.UpperBound)
            .FirstOrDefault(r => r.UpperBound > income)
            ?? throw new Exception($"No applicable rule is found in {year} for {income}");

        return applicableRule.Rate * income / 100 
             + applicableRule.FixedPayment;
    }   
}