using Microsoft.AspNetCore.Mvc;
using TaxCalculator.Seed.Calculators;

namespace TaxCalculator.Seed.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaxCalculatorController : ControllerBase
{
    private readonly ICalculator _calculator;
    public TaxCalculatorController(ICalculator calculator) => _calculator = calculator;

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
}