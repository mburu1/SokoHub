using MediatR;
using SokoHub.Contracts.Catalog;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Common.Guards;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Application.Modules.Catalog;

public record CreateCategoryCommand(
    string Name,
    string Description,
    Guid? ParentCategoryId = null) : IRequest<CategoryResponse>;

public sealed class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = Category.Create(request.Name, request.ParentCategoryId);

        await _unitOfWork.Repository<Category>().AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CategoryResponse(
            category.Id,
            category.Name,
            category.Slug.Value,
            string.Empty,
            category.ParentId);
    }
}
