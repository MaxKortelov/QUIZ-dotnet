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
}