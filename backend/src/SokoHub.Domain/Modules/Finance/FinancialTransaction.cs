using SokoHub.Domain.Common.Entities;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Domain.Modules.Finance;

public class FinancialTransaction : Entity
{
    public Guid AccountId { get; private set; }
    public Money Amount { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public string Description { get; private set; }
    public Guid RelatedTransactionId { get; private set; }

    public FinancialTransaction(Guid id, Guid accountId, Money amount, string description, Guid? relatedTransactionId = null)
        : base(id)
    {
        AccountId = accountId;
        Amount = amount;
        Description = description;
        RelatedTransactionId = relatedTransactionId ?? Guid.Empty;
        TransactionDate = DateTime.UtcNow;
    }
}
