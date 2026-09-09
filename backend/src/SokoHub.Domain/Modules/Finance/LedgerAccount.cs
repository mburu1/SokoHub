using SokoHub.Domain.Common.AggregateRoots;

namespace SokoHub.Domain.Modules.Finance;

public class LedgerAccount : AggregateRoot
{
    public string AccountNumber { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string AccountType { get; private set; } = null!; // e.g. "Asset", "Liability", "Equity", "Revenue", "Expense"
    public decimal Balance { get; private set; }

    private LedgerAccount() { }

    public LedgerAccount(Guid id, string accountNumber, string name, string accountType)
        : base(id)
    {
        AccountNumber = accountNumber;
        Name = name;
        AccountType = accountType;
        Balance = 0;
    }

    public void UpdateBalance(decimal amount)
    {
        Balance += amount;
        Touch();
    }
}
