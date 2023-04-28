using Microsoft.Extensions.DependencyInjection.Extensions;
using TaxCalculator.App.Calculators;
using TaxCalculator.App.RuleProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.TryAddScoped<ICalculator, Calculator>();
builder.Services.TryAddScoped<IRangeCalculator, RangeCalculator>();

builder.Services.TryAddScoped<TaxationRuleRepository>();
builder.Services.TryAddScoped<InMemoryTaxationRuleProvider>();
builder.Services.TryAddScoped<ITaxationRuleProvider>(sp =>
{
    var providers = new ITaxationRuleProvider[]
    {
        sp.GetRequiredService<TaxationRuleRepository>(),
        sp.GetRequiredService<InMemoryTaxationRuleProvider>(),
    };

    //return ActivatorUtilities.CreateInstance<CompositeTaxationRuleProvider>(sp, providers);
    return new CompositeTaxationRuleProvider(providers);
});

builder.Services.TryAddScoped<ITaxationRuleStorage>(sp => 
    sp.GetRequiredService<TaxationRuleRepository>());

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

namespace TaxCalculator.App
{
    public partial class Program { }
}
