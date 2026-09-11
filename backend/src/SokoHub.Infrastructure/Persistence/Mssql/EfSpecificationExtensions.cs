using Microsoft.EntityFrameworkCore;
using SokoHub.Domain.Common.Specifications;

namespace SokoHub.Infrastructure.Persistence.Mssql;

public static class EfSpecificationExtensions
{
    public static IQueryable<T> ApplySpecification<T>(this ISpecification<T> spec, DbSet<T> dbSet)
        where T : class
        => spec.ApplySpecification(dbSet.AsQueryable());
}