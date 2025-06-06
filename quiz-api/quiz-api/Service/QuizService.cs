public interface IQuizService
{
    Task<List<QuestionTypeDto>> GetQuizTypesAsync();
    Task<QuizSessionDto> GenerateQuizSession(Guid quizTypeId, Guid userId);
}

public class QuizService : IQuizService
{
    private readonly QuizRepository _quizRepository;
    private readonly EnvVars _envVars;

    public QuizService(QuizRepository quizRepository, EnvVars envVars)
    {
        _quizRepository = quizRepository;
        _envVars = envVars;
    }

    public async Task<List<QuestionTypeDto>> GetQuizTypesAsync()
    {
        return await _quizRepository.GetQuestionTypeListAsync();
    }

    public async Task<QuizSessionDto?> GenerateQuizSession(Guid quizTypeId, Guid userId)
    {
        var emptyQuizSession = await _quizRepository.FindEmptyQuizSessionAsync(quizTypeId, userId);
        Console.WriteLine(emptyQuizSession?.ToString());
        if (emptyQuizSession != null)
        {
            return new QuizSessionDto(emptyQuizSession);
        }
        
        var quizQuestions = await _quizRepository.GetQuizQuestionsAsync(quizTypeId);
        var questionSequence = quizQuestions
            .Select(q => q.Uuid)
            .OrderBy(_ => Guid.NewGuid()) // Random order
            .ToList();
        var duration = questionSequence.Count * _envVars.TimePerQuestion;
        var attempts = _envVars.AttemptsPerQuiz;
        
        var quizSession =
            await _quizRepository.AddQuizSessionAsync(quizTypeId, userId, questionSequence, duration, attempts);
        
        return new QuizSessionDto(quizSession);
    }
}