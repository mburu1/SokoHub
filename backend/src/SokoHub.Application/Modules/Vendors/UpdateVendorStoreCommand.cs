using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Vendors;

public record UpdateVendorStoreCommand(
    Guid StoreId,
    string StoreName,
    string Description,
    string LogoUrl,
    string BannerUrl) : IRequest<Result>;

public sealed class UpdateVendorStoreHandler : IRequestHandler<UpdateVendorStoreCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public UpdateVendorStoreHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateVendorStoreCommand request, CancellationToken cancellationToken)
    {
        var store = await _unitOfWork.Repository<VendorStore>().GetByIdAsync(request.StoreId, cancellationToken);

        if (store == null)
        {
            return Result.Failure(new ApplicationError("store_not_found", "Store not found."));
        }

        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(store.VendorId, cancellationToken);

        if (vendor == null || vendor.UserId != _currentUser.Id)
        {
            return Result.Failure(new ApplicationError("unauthorized", "You can only update your own store."));
        }

        store.UpdateProfile(request.StoreName, request.Description, request.LogoUrl, request.BannerUrl);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
