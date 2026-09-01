using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Vendors;

public record UpdateVendorStoreCommand(
    Guid StoreId,
    string StoreName,
    string Description,
    string LogoUrl,
    string BannerUrl) : IRequest<bool>;

public sealed class UpdateVendorStoreHandler : IRequestHandler<UpdateVendorStoreCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public UpdateVendorStoreHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(UpdateVendorStoreCommand request, CancellationToken cancellationToken)
    {
        var store = await _unitOfWork.Repository<VendorStore>().GetByIdAsync(request.StoreId, cancellationToken);

        if (store == null)
        {
            throw new KeyNotFoundException("Store not found.");
        }

        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(store.VendorId, cancellationToken);

        if (vendor == null || vendor.UserId != _currentUser.Id)
        {
            throw new UnauthorizedAccessException("You can only update your own store.");
        }

        store.UpdateProfile(request.StoreName, request.Description, request.LogoUrl, request.BannerUrl);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
