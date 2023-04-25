namespace TaxCalculator.App.Entities;

public class RangeCalculationModel
{
    public IReadOnlyDictionary<int, decimal> IncomeByYear { get; init; }
        = new Dictionary<int, decimal>();
    public IReadOnlyDictionary<int, TaxationRule[]> RulesByYear { get; init; } 
        = new Dictionary<int, TaxationRule[]>();
}