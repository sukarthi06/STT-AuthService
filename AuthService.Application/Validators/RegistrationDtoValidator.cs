namespace AuthService.Application.Validators;

public class RegistrationDtoValidator : AbstractValidator<RegistrationDto>
{
    public RegistrationDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.FirstName).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.LicenseNo).MaximumLength(50);

        // Organization license is required only when an organization name is given
        RuleFor(x => x.OrganizationLicenseNo)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.OrganizationName));
    }
}