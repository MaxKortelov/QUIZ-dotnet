using System.Text.Json;
using Microsoft.EntityFrameworkCore;

public class QuizRepository
{
    private readonly QuizDbContext _context;

    public QuizRepository(QuizDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<QuestionTypeDto>> GetQuestionTypeListAsync()
    {
        return await _context.QuestionTypes
            .Select(qt => new QuestionTypeDto
            {
                Uuid = qt.Uuid,
                Description = qt.Description
            })
            .ToListAsync();
    }
    
    public async Task<QuizTableResult> CreateUserQuizTableResultsAsync(Guid userId)
    {
        var quizTableResult = new QuizTableResult
        {
            UserId = userId,
            QuizAmountTaken = 0,
            CorrectAnswers = 0,
        };

        _context.QuizTableResults.Add(quizTableResult);
        await _context.SaveChangesAsync();

        return quizTableResult;
    }
    
    public async Task<QuizTableResult> GetUserQuizTableResultAsync(Guid userId)
    {
        var quizTableResult = await _context.QuizTableResults.FirstOrDefaultAsync(u => u.UserId == userId);

        if (quizTableResult == null)
        {
            var createdQuizTableResult = await CreateUserQuizTableResultsAsync(userId);
            
            return createdQuizTableResult;
        }
        
        return quizTableResult;
    }
    
    public async Task<QuizSession?> FindEmptyQuizSessionAsync(Guid quizTypeId, Guid userId)
    {
        return await _context.QuizSessions
            .Where(qs => qs.QuestionTypeId == quizTypeId && qs.UserId == userId && qs.DateEnded == null)
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<Question>> GetQuizQuestionsAsync(Guid quizTypeId)
    {
        return await _context.Questions
            .Where(q => q.QuestionTypeId == quizTypeId)
            .ToListAsync();
    }
    
    public async Task<QuizSession> AddQuizSessionAsync(Guid quizTypeId, Guid userId, List<Guid> questionSequence, int duration, int attempts)
    {
        var quizSession = new QuizSession
        {
            QuestionSequence = questionSequence,
            QuestionTypeId = quizTypeId,
            UserId = userId,
            Duration = duration,
            Attempts = attempts,
            DateCreated = DateTime.UtcNow
        };

        _context.QuizSessions.Add(quizSession);
        await _context.SaveChangesAsync();

        return quizSession;
    }
    
    public async Task<QuizSession> GetQuizSessionAsync(Guid quizSessionId, Guid userId)
    {
        var quiz = await _context.QuizSessions
            .FirstOrDefaultAsync(q => q.Uuid == quizSessionId && q.UserId == userId);

        if (quiz == null)
        {
            throw new NotFoundException("Quiz session not found.");
        }

        return quiz;
    }
    
    public async Task StartQuizSessionAsync(Guid quizSessionId, Guid userId)
    {
        var quizSession = await GetQuizSessionAsync(quizSessionId, userId);
        
        if (quizSession == null)
            throw new NotFoundException("Quiz session not found.");

        if (quizSession.Attempts - quizSession.AttemptsUsed <= 0)
            throw new BadRequestException("All attempts are used");

        quizSession.DateStarted = DateTime.UtcNow;
        quizSession.DateEnded = DateTime.UtcNow.AddMinutes(quizSession.Duration);
        quizSession.AttemptsUsed += 1;

        await _context.SaveChangesAsync();
    }
    
    public async Task<QuestionType> GetQuizTypeByIdAsync(Guid questionTypeId)
    {
        var questionType = await _context.QuestionTypes
            .FirstOrDefaultAsync(q => q.Uuid == questionTypeId);

        if (questionType == null)
        {
            throw new NotFoundException("QuestionType not found.");
        }

        return questionType;
    }
    
    public async Task<Question> GetQuizQuestionAsync(Guid questionId)
    {
        var question = await _context.Questions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Uuid == questionId);

        if (question == null)
        {
            throw new NotFoundException("Question not found.");
        }

        return question;
    }

    public async Task AddQuestionAnswerAsync(SaveQuizQuestionDto saveQuizSessionDto, Guid userId)
    {
        var quizSession = await GetQuizSessionAsync(saveQuizSessionDto.QuizSessionId, userId);
        if (quizSession == null)
            throw new NotFoundException("Quiz session not found.");

        var questionAnswerDict = string.IsNullOrWhiteSpace(quizSession.QuestionAnswer)
            ? new Dictionary<Guid, Guid>()
            : JsonSerializer.Deserialize<Dictionary<Guid, Guid>>(quizSession.QuestionAnswer)!;

        questionAnswerDict[saveQuizSessionDto.QuestionId] = saveQuizSessionDto.AnswerId;

        quizSession.QuestionAnswer = JsonSerializer.Serialize(questionAnswerDict);

        await _context.SaveChangesAsync();
    }
    
    private async Task<List<Question>> GetQuestionsByTypeIdAsync(Guid questionTypeId)
    {
        return await _context.Questions
            .Where(q => q.QuestionTypeId == questionTypeId)
            .ToListAsync();
    }
    
    public async Task<(int correctAnswersCount, string resultInPercentage)> SaveAndCountQuizResultAsync(Guid quizSessionId, Guid userId)
    {
        var quizSession = await GetQuizSessionAsync(quizSessionId, userId);

        if (quizSession == null)
        {
            throw new NotFoundException("Quiz session not found.");
        }

        var questions = await GetQuestionsByTypeIdAsync(quizSession.QuestionTypeId.Value);

        var questionAnswerDict = string.IsNullOrWhiteSpace(quizSession.QuestionAnswer)
            ? new Dictionary<string, string>()
            : JsonSerializer.Deserialize<Dictionary<string, string>>(quizSession.QuestionAnswer)!;

        var answersCheckList = questions.Select(q =>
            questionAnswerDict.TryGetValue(q.Uuid.ToString(), out var answerId) && answerId == q.CorrectAnswers.FirstOrDefault()
        ).ToList();

        var correctAnswersCount = answersCheckList.Count(c => c);
        var result = answersCheckList.Count > 0 
            ? 100.0 * correctAnswersCount / answersCheckList.Count 
            : 0;

        quizSession.Result = (int)result;
        quizSession.DateEnded = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (correctAnswersCount, $"{result}%");
    }

    public async Task UpdateUserQuizTableResultsAsync(Guid userId, Guid quizSessionId,
        int quizSessionCorrectAnswersCount)
    {
        var currentQuizTableResults = await GetUserQuizTableResultAsync(userId);
        var oldQuizSession = currentQuizTableResults.BestQuizSessionId != null
            ? await GetQuizSessionAsync(currentQuizTableResults.BestQuizSessionId.Value, userId)
            : null;
        var newQuizSession = await GetQuizSessionAsync(quizSessionId, userId);
        
        var bestQuizSession = CalculateBestQuizSession(oldQuizSession, newQuizSession);

        currentQuizTableResults.BestQuizSessionId = bestQuizSession.Uuid;
        currentQuizTableResults.QuizAmountTaken += 1;
        currentQuizTableResults.CorrectAnswers += quizSessionCorrectAnswersCount;
        currentQuizTableResults.BestQuizSession = bestQuizSession;
        
        await _context.SaveChangesAsync();
    }
    
    private QuizSession CalculateBestQuizSession(QuizSession? oldQuizSession, QuizSession? newQuizSession)
    {
        if (oldQuizSession == null && newQuizSession != null)
        {
            return newQuizSession;
        }

        if (newQuizSession == null && oldQuizSession != null)
        {
            return oldQuizSession;
        }

        if (oldQuizSession != null && newQuizSession != null)
        {
            var bestTimeOldQuizSession = (oldQuizSession.DateEnded - oldQuizSession.DateStarted)?.TotalSeconds ?? double.MaxValue;
            var bestTimeNewQuizSession = (newQuizSession.DateEnded - newQuizSession.DateStarted)?.TotalSeconds ?? double.MaxValue;

            return bestTimeOldQuizSession < bestTimeNewQuizSession ? oldQuizSession : newQuizSession;
        }

        throw new Exception("calculateBestQuizSession - Quiz session wasn't found");
    }
}