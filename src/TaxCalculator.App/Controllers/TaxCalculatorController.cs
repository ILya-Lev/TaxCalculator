using Microsoft.AspNetCore.Mvc;
using TaxCalculator.App.Calculators;
using TaxCalculator.App.Entities;
using TaxCalculator.App.RuleProviders;

namespace TaxCalculator.App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaxCalculatorController : ControllerBase
{
    private readonly ICalculator _calculator;
    private readonly IRangeCalculator _rangeCalculator;
    private readonly ITaxationRuleStorage _ruleStorage;

    public TaxCalculatorController(ICalculator calculator
        , IRangeCalculator rangeCalculator
        , ITaxationRuleStorage ruleStorage)
    {
        _calculator = calculator;
        _rangeCalculator = rangeCalculator;
        _ruleStorage = ruleStorage;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Calculate([FromQuery] decimal income, [FromQuery] int year)
    {
        await Task.Yield();

        try
        {
            var amount = _calculator.GetTaxAmount(income, year);
            return Ok(amount);
        }
        catch (Exception exc)
        {
            return BadRequest(exc.Message);
        }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> CalculateForRange(
        [FromBody] RangeCalculationModel calculationModel)
    {
        await Task.Yield();
        _ruleStorage.Store(calculationModel.RulesByYear);
        var taxByYear = _rangeCalculator.GetTaxAmounts(calculationModel.IncomeByYear);
        return Ok(taxByYear);
    }
}