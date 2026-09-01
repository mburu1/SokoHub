using FluentValidation;
using SokoHub.Application.Modules.Vendors;

namespace SokoHub.Application.Modules.Vendors.Validators;

public class ApproveVendorValidator : AbstractValidator<ApproveVendorCommand>
{
    public ApproveVendorValidator()
    {
        RuleFor(x => x.VendorId).NotEmpty();
        RuleFor(x => x.ApprovedBy).NotEmpty();
    }
}
