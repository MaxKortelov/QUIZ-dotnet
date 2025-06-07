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
            .Include(q => q.Answers) // eager load answers
            .FirstOrDefaultAsync(q => q.Uuid == questionId);

        if (question == null)
        {
            throw new NotFoundException("Question not found.");
        }

        return question;
    }
}