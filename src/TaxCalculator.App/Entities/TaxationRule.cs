namespace TaxCalculator.App.Entities;

public record TaxationRule(decimal UpperBound, decimal FixedPayment, decimal Rate);
