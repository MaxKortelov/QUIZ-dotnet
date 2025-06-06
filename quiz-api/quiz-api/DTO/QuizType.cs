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