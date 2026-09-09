using SokoHub.Domain.Common.AggregateRoots;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Domain.Modules.Finance;

public class FinancialPeriod : AggregateRoot
{
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsClosed { get; private set; }
    public string PeriodName { get; private set; } = null!;

    private FinancialPeriod() { }

    public FinancialPeriod(Guid id, string periodName, DateTime startDate, DateTime endDate)
        : base(id)
    {
        PeriodName = periodName;
        StartDate = startDate;
        EndDate = endDate;
        IsClosed = false;
    }

    public void Close()
    {
        IsClosed = true;
        Touch();
    }

    public void Open()
    {
        IsClosed = false;
        Touch();
    }
}
