using FluentValidation;
using SokoHub.Application.Modules.Vendors;

namespace SokoHub.Application.Modules.Vendors.Validators;

public class SubmitKycValidator : AbstractValidator<SubmitKycCommand>
{
    public SubmitKycValidator()
    {
        RuleFor(x => x.VendorId).NotEmpty();
        RuleFor(x => x.DocumentType).NotEmpty();
        RuleFor(x => x.DocumentUrl).NotEmpty();
        RuleFor(x => x.Checksum).NotEmpty();
    }
}
