using System.Linq.Expressions;
using SokoHub.Domain.Interfaces;

namespace SokoHub.Domain.Common.Specifications;

public abstract class Specification<T> : ISpecification<T>
{
    public abstract Expression<Func<T, bool>> Criteria { get; }

    public List<Expression<Func<T, object>>> Includes { get; } = [];

    public Expression<Func<T, object>>? OrderBy { get; private set; }

    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    public int? Take { get; private set; }

    public int? Skip { get; private set; }

    public bool IsPagingEnabled => Take is not null;

    public Specification<T> AddInclude(Expression<Func<T, object>> include)
    {
        Includes.Add(include);
        return this;
    }

    public Specification<T> ApplyPaging(int skip, int take)
    {
        Skip = Ensure.NotNegative(skip);
        Take = Ensure.Positive(take);
        return this;
    }

    public Specification<T> ApplyOrderBy(Expression<Func<T, object>> orderBy)
    {
        OrderBy = orderBy;
        return this;
    }

    public Specification<T> ApplyOrderByDescending(Expression<Func<T, object>> orderByDescending)
    {
        OrderByDescending = orderByDescending;
        return this;
    }

    public Specification<T> And(Specification<T> other) => new CombinedSpecification<T>(this, other, CombineMode.And);

    public Specification<T> Or(Specification<T> other) => new CombinedSpecification<T>(this, other, CombineMode.Or);

    private enum CombineMode
    {
        And,
        Or
    }

    private sealed class CombinedSpecification<TEntity> : Specification<TEntity>
    {
        private readonly Specification<TEntity> _left;
        private readonly Specification<TEntity> _right;
        private readonly CombineMode _mode;

        public CombinedSpecification(Specification<TEntity> left, Specification<TEntity> right, CombineMode mode)
        {
            _left = left;
            _right = right;
            _mode = mode;
        }

        public override Expression<Func<TEntity, bool>> Criteria
        {
            get
            {
                var parameter = Expression.Parameter(typeof(TEntity), "x");
                var left = ReplaceParameter(_left.Criteria, parameter);
                var right = ReplaceParameter(_right.Criteria, parameter);
                var body = _mode == CombineMode.And
                    ? Expression.AndAlso(left, right)
                    : Expression.OrElse(left, right);
                return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
            }
        }

        private static Expression ReplaceParameter(LambdaExpression lambda, ParameterExpression parameter)
        {
            return new ParameterReplacer(lambda.Parameters[0], parameter).Visit(lambda.Body)
                   ?? throw new InvalidOperationException("Failed to compose specification.");
        }
    }

    private sealed class ParameterReplacer(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) =>
            node == source ? target : base.VisitParameter(node);
    }
}
