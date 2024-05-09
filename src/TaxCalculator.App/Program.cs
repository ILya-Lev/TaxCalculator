using Microsoft.Extensions.DependencyInjection.Extensions;
using TaxCalculator.App.Calculators;
using TaxCalculator.App.RuleProviders;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseDefaultServiceProvider(p =>
{
    p.ValidateOnBuild = true;
    p.ValidateScopes = true;
});

builder.Services.TryAddScoped<ICalculator, Calculator>();
builder.Services.TryAddScoped<IRangeCalculator, RangeCalculator>();

builder.Services.TryAddScoped<ITaxationRuleProvider, InMemoryTaxationRuleProvider>();
builder.Services.Decorate<ITaxationRuleProvider, TaxationRuleRepository>();
builder.Services.TryAddScoped<ITaxationRuleStorage>(sp => 
    (sp.GetRequiredService<ITaxationRuleProvider>() as ITaxationRuleStorage)!);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

namespace TaxCalculator.App
{
    public partial class Program { }
}
