using SokoHub.Domain.Common.AggregateRoots;
using SokoHub.Domain.Common.Entities;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Domain.Modules.Finance;

public class JournalEntry : AggregateRoot
{
    public DateTime EntryDate { get; private set; }
    public string Description { get; private set; } = null!;
    public string Reference { get; private set; } = null!;
    private readonly List<JournalEntryLine> _lines = [];

    public IReadOnlyList<JournalEntryLine> Lines => _lines.AsReadOnly();

    private JournalEntry() { }

    public JournalEntry(Guid id, string description, string reference, DateTime entryDate)
        : base(id)
    {
        Description = description;
        Reference = reference;
        EntryDate = entryDate;
    }

    public void AddLine(Guid accountId, Money amount, bool isDebit)
    {
        var line = new JournalEntryLine(Guid.NewGuid(), this.Id, accountId, amount, isDebit);
        _lines.Add(line);
        Touch();
    }

    public bool IsBalanced()
    {
        var debits = _lines.Where(l => l.IsDebit).Sum(l => l.Value.Value);
        var credits = _lines.Where(l => !l.IsDebit).Sum(l => l.Value.Value);
        return Math.Abs(debits - credits) < 0.01m;
    }
}
