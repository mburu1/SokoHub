using SokoHub.Domain.Common.Entities;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Domain.Modules.Finance;

public class JournalEntryLine : Entity
{
    public Guid JournalEntryId { get; private set; }
    public Guid AccountId { get; private set; }
    public Money Amount { get; private set; }
    public bool IsDebit { get; private set; }

    public JournalEntryLine(Guid id, Guid journalEntryId, Guid accountId, Money amount, bool isDebit)
        : base(id)
    {
        JournalEntryId = journalEntryId;
        AccountId = accountId;
        Amount = amount;
        IsDebit = isDebit;
    }
}
