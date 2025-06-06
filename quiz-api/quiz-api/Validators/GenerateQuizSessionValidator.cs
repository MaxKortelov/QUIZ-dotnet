using FluentValidation;

public class GenerateQuizSessionValidator : AbstractValidator<GenerateQuizSessionDto>
{
    public GenerateQuizSessionValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.QuizTypeId).NotEmpty();
    }
}