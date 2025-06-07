using System.Text.Json;

public interface IQuizService
{
    Task<List<QuestionTypeDto>> GetQuizTypesAsync();
    Task<QuizSessionDto> GenerateQuizSession(Guid quizTypeId, Guid userId);
    Task<QuizDataDto> InitiateQuizSessionAsync(Guid quizSessionId, Guid userId);
    Task<QuizQustionWithAnswersDto> FindNextQuizQuestionAsync(Guid quizSessionId, Guid userId);
    Task AddQuizQuestionAnswerAsync(SaveQuizQuestionDto saveQuizSessionDto, Guid userId);
    Task<QuizSession> GetQuizSessionAsync(SaveQuizQuestionDto saveQuizSessionDto, Guid userId);
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

    public async Task<QuizQustionWithAnswersDto> FindNextQuizQuestionAsync(Guid quizSessionId, Guid userId)
    {
        var quizSession = await _quizRepository.GetQuizSessionAsync(quizSessionId, userId);
        
        if (quizSession == null)
        {
            throw new NotFoundException("QuizSession is not found");
        }
        
        var answeredQuestions = quizSession.QuestionAnswer != null
            ? JsonSerializer.Deserialize<Dictionary<string, object>>(quizSession.QuestionAnswer).Keys.ToList()
            : new List<string>();
        
        var currentQuestionId = quizSession.QuestionSequence
            .FirstOrDefault(q => !answeredQuestions.Contains(q.ToString()));
        
        var quizTypeDto = await _quizRepository.GetQuizTypeByIdAsync(quizSession.QuestionTypeId.Value); // if nullable Guid
        var description = quizTypeDto.Description;

        var question = await _quizRepository.GetQuizQuestionAsync(currentQuestionId);

        return new QuizQustionWithAnswersDto(question, description);
    }

    public async Task<QuizDataDto> InitiateQuizSessionAsync(Guid quizSessionId, Guid userId)
    {
        await _quizRepository.StartQuizSessionAsync(quizSessionId, userId);
        
        var question = await FindNextQuizQuestionAsync(quizSessionId, userId);
        if (question == null)
        {
            throw new NotFoundException("Quiz is not valid");
        }
        var quizSession = await _quizRepository.GetQuizSessionAsync(quizSessionId, userId);
        
        if (quizSession == null)
        {
            throw new NotFoundException("QuizSession is not found");
        }

        return new QuizDataDto(question, quizSession);
    }

    public async Task AddQuizQuestionAnswerAsync(SaveQuizQuestionDto saveQuizSessionDto, Guid userId)
    {
        await _quizRepository.AddQuestionAnswerAsync(saveQuizSessionDto, userId);
    }
    
    public async Task<QuizSession> GetQuizSessionAsync(SaveQuizQuestionDto saveQuizSessionDto, Guid userId)
    {
        return await _quizRepository.GetQuizSessionAsync(saveQuizSessionDto.QuizSessionId, userId);
    }
}