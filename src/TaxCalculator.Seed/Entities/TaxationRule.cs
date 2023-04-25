namespace TaxCalculator.Seed.Entities;

public record TaxationRule(decimal UpperBound, decimal FixedPayment, decimal Rate);
