using FluentValidation;

public class SubmitQuizSessionValidator : AbstractValidator<SubmitQuizSessionDto>
{
    public SubmitQuizSessionValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.QuizSessionId).NotEmpty();
    }
}