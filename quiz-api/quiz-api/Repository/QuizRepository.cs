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
            .Where(qs => qs.QuestionTypeId == quizTypeId && qs.UserId == userId)
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
            // QuestionAnswer =
            DateCreated = DateTime.UtcNow
        };

        _context.QuizSessions.Add(quizSession);
        await _context.SaveChangesAsync();

        return quizSession;
    }
}