namespace TaxCalculator.App.Calculators;

public interface IRangeCalculator
{
    IReadOnlyDictionary<int, decimal?> GetTaxAmounts(IReadOnlyDictionary<int, decimal> incomeByYear);
}

public class RangeCalculator : IRangeCalculator
{
    private readonly ICalculator _calculator;
    public RangeCalculator(ICalculator calculator) => _calculator = calculator;

    public IReadOnlyDictionary<int, decimal?> GetTaxAmounts(
        IReadOnlyDictionary<int, decimal> incomeByYear)
        => incomeByYear.ToDictionary(p => p.Key, p =>
        {
            try
            {
                return _calculator.GetTaxAmount(p.Value, p.Key);
            }
            catch
            {
                return default(decimal?);
            }
        });
}