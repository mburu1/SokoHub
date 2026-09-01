using FluentValidation;
using SokoHub.Application.Modules.Vendors;

namespace SokoHub.Application.Modules.Vendors.Validators;

public class RejectVendorValidator : AbstractValidator<RejectVendorCommand>
{
    public RejectVendorValidator()
    {
        RuleFor(x => x.VendorId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty();
        RuleFor(x => x.RejectedBy).NotEmpty();
    }
}
