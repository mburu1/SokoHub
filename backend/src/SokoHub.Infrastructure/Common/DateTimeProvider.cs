using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Infrastructure.Common;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.Now;

    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
