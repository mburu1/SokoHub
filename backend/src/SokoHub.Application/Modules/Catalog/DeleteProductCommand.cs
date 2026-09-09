using MediatR;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Catalog;

public record DeleteProductCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public DeleteProductHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
        {
            return Result.Failure(new ApplicationError("product_not_found", "Product not found."));
        }

        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(product.VendorId, cancellationToken);
        if (vendor == null || vendor.UserId != _currentUser.Id)
        {
            return Result.Failure(new ApplicationError("unauthorized", "You are not authorized to delete this product."));
        }

        await _unitOfWork.Repository<Product>().DeleteAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
