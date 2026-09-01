using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Vendors;

public record CreateVendorStoreCommand(
    Guid VendorId,
    string StoreName,
    string Description,
    string LogoUrl,
    string BannerUrl) : IRequest<Guid>;

public sealed class CreateVendorStoreHandler : IRequestHandler<CreateVendorStoreCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public CreateVendorStoreHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateVendorStoreCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(request.VendorId, cancellationToken);

        if (vendor == null)
        {
            throw new KeyNotFoundException("Vendor not found.");
        }

        if (vendor.UserId != _currentUser.Id)
        {
            throw new UnauthorizedAccessException("You can only create a store for your own vendor account.");
        }

        if (vendor.Status != VendorStatus.Active)
        {
            throw new InvalidOperationException("Only active vendors can create a store.");
        }

        var store = new VendorStore(
            Guid.NewGuid(),
            vendor.Id,
            request.StoreName,
            request.Description,
            request.LogoUrl,
            request.BannerUrl);

        await _unitOfWork.Repository<VendorStore>().AddAsync(store, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return store.Id;
    }
}
