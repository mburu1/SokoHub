namespace SokoHub.Domain.Modules.Payments;

public sealed class PaymentReconciliation : AggregateRoot
{
    private PaymentReconciliation()
    {
    }

    private PaymentReconciliation(Guid id, DateOnly statementDate, Money expected, Money actual)
        : base(id)
    {
        StatementDate = statementDate;
        ExpectedTotal = expected;
        ActualTotal = actual;
        Variance = new Money(Math.Abs(expected.Amount - actual.Amount), expected.Currency);
        IsBalanced = expected.Amount == actual.Amount;
    }

    public DateOnly StatementDate { get; private set; }

    public Money ExpectedTotal { get; private set; }

    public Money ActualTotal { get; private set; }

    public Money Variance { get; private set; }

    public bool IsBalanced { get; private set; }

    public static PaymentReconciliation FromStatement(DateOnly statementDate, Money expected, Money actual)
    {
        Ensure.That(expected.Currency == actual.Currency, "currency_mismatch", "Statement currency mismatch.");
        return new PaymentReconciliation(Guid.Empty, statementDate, expected, actual);
    }
}
