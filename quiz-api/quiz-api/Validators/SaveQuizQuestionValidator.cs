using FluentValidation;

public class SaveQuizQuestionValidator : AbstractValidator<SaveQuizQuestionDto>
{
    public SaveQuizQuestionValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.QuizSessionId).NotEmpty();
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.AnswerId).NotEmpty();
    }
}