public interface IQuizService
{
    Task<List<QuestionTypeDto>> GetQuizTypesAsync();
}

public class QuizService : IQuizService
{
    private readonly QuizRepository _quizRepository;

    public QuizService(QuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public async Task<List<QuestionTypeDto>> GetQuizTypesAsync()
    {
        return await _quizRepository.GetQuestionTypeListAsync();
    }
}