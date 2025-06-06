using FluentValidation;

public class LoadUserValidator : AbstractValidator<LoadUserDto>
{
    public LoadUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}