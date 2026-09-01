using MediatR;
using SokoHub.Contracts.Catalog;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Application.Modules.Catalog;

public record CreateBrandCommand(
    string Name,
    string Description) : IRequest<BrandResponse>;

public sealed class CreateBrandHandler : IRequestHandler<CreateBrandCommand, BrandResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBrandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BrandResponse> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = new Brand(
            Guid.NewGuid(),
            Ensure.NotBlank(request.Name),
            Slug.From(request.Name),
            Ensure.MaxLength(request.Description, 1000));

        await _unitOfWork.Repository<Brand>().AddAsync(brand, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BrandResponse(
            brand.Id,
            brand.Name,
            brand.Slug.Value,
            brand.Description);
    }
}
