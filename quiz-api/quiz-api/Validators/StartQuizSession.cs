using FluentValidation;

public class StartQuizSessionValidator : AbstractValidator<StartQuizSessionDto>
{
    public StartQuizSessionValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.QuizSessionId).NotEmpty();
    }
}