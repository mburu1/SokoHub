using FluentValidation;
using SokoHub.Application.Modules.Vendors;

namespace SokoHub.Application.Modules.Vendors.Validators;

public class RegisterVendorValidator : AbstractValidator<RegisterVendorCommand>
{
    public RegisterVendorValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.BusinessName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TaxId).NotEmpty();
        RuleFor(x => x.CommissionRate).InclusiveBetween(0, 100);
    }
}
