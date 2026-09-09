using MediatR;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Catalog;

public record ApproveProductCommand(Guid ProductId) : IRequest<Result>;

public sealed class ApproveProductHandler : IRequestHandler<ApproveProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public ApproveProductHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ApproveProductCommand request, CancellationToken cancellationToken)
    {
        // Ensure user is admin
        if (!_currentUser.IsAdmin)
        {
            return Result.Failure(new ApplicationError("unauthorized", "Only administrators can approve products."));
        }

        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
        {
            return Result.Failure(new ApplicationError("product_not_found", "Product not found."));
        }

        // Logic for approval (e.g. changing status to Approved or Active)
        // For now we just assume publishing it is the approval.
        product.Publish();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
