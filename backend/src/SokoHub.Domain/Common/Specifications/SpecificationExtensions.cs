using System.Linq;
using System.Linq.Expressions;
using SokoHub.Domain.Interfaces;

namespace SokoHub.Domain.Common.Specifications;

public static class SpecificationExtensions
{
    public static IQueryable<T> ApplySpecification<T>(this ISpecification<T> spec, IQueryable<T> query)
    {
        var result = query;

        if (spec.Criteria is not null)
        {
            result = result.Where(spec.Criteria);
        }

        if (spec.OrderBy is not null)
        {
            result = result.OrderBy(spec.OrderBy);
        }

        if (spec.OrderByDescending is not null)
        {
            result = result.OrderByDescending(spec.OrderByDescending);
        }

        if (spec.Skip.HasValue && spec.Take.HasValue)
        {
            result = result.Skip(spec.Skip.Value).Take(spec.Take.Value);
        }

        return result;
    }
}