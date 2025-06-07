using System;
using System.Globalization;

public class GenerateQuizSessionDto
{
    public required string Email { get; set; }
    public required Guid QuizTypeId { get; set; }
}

public class QuizSessionDto
{
    public Guid QuizSessionId { get; set; }
    public int QuestionAmount { get; set; }
    public int QuizDuration { get; set; }
    public int QuizAttempts { get; set; }
    public int QuizAttemptsUsed { get; set; }
    public string DateCreated { get; set; }

    public QuizSessionDto(QuizSession quizSession)
    {
        QuizSessionId = quizSession.Uuid;
        DateCreated = quizSession.DateCreated.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.000Z");
        QuestionAmount = quizSession.QuestionSequence.Count;
        QuizDuration = quizSession.Duration;
        QuizAttempts = quizSession.Attempts;
        QuizAttemptsUsed = quizSession.AttemptsUsed;
    }
}

public record QuizSessionRecord(QuizSessionDto QuizSession) {}

public class StartQuizSessionDto
{
    public required string Email { get; set; }
    public required Guid QuizSessionId { get; set; }
}

public class SubmitQuizSessionDto
{
    public required string Email { get; set; }
    public required Guid QuizSessionId { get; set; }
}

public class QuizDataDto
{
    public QuizQustionWithAnswersDto Question { get; set; }
    public DateTime DateStarted { get; set; }
    public DateTime DateEnded { get; set; }
    public int CurrentQuestionCount { get; set; }
    public int QuestionsAmount { get; set; }

    public QuizDataDto(QuizQustionWithAnswersDto question, QuizSession quizSession)
    {
        Question = question;
        DateStarted = quizSession.DateStarted ?? DateTime.UtcNow;
        DateEnded = quizSession.DateEnded ?? DateTime.UtcNow;
        CurrentQuestionCount = quizSession.QuestionSequence.FindIndex(q => q == question.QuestionId) + 1;
        QuestionsAmount = quizSession.QuestionSequence.Count;
    }
}

public record QuizAnswerDto(Guid Id, string Text) {}

public record QuizQustionWithAnswersDto
{
    public Guid QuestionId { get; init; }
    public string Question { get; init; }
    public List<QuizAnswerDto> Answers { get; init; }
    public string QuizType { get; init; }

    public QuizQustionWithAnswersDto(Question question, string quizType)
    {
        QuestionId = question.Uuid;
        Question = question.QuestionText;
        Answers = question.Answers
            .Select(a => new QuizAnswerDto(a.Uuid, a.AnswerText))
            .ToList();
        QuizType = quizType;
    }
}

public class SaveQuizQuestionDto
{
    public required string Email { get; set; }
    public required Guid QuizSessionId { get; set; }
    public required Guid QuestionId { get; set; }
    public required Guid AnswerId { get; set; }
}

public record SubmitQuizSessionResultResponseDto(Guid QuizSessionId, string Result);