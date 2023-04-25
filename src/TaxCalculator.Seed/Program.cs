using Microsoft.Extensions.DependencyInjection.Extensions;
using TaxCalculator.Seed.Calculators;
using TaxCalculator.Seed.RuleProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.TryAddScoped<ICalculator, Calculator>();

builder.Services.TryAddScoped<InMemoryTaxationRuleProvider>();
builder.Services.TryAddScoped<ITaxationRuleProvider, InMemoryTaxationRuleProvider>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.ConfigObject.DisplayRequestDuration = true;
        opt.ConfigObject.TryItOutEnabled = true;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

namespace TaxCalculator.Seed
{
    public partial class Program { }
}
