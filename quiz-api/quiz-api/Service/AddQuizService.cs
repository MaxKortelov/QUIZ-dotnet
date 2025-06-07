using System.Text.Json;

public interface IAddQuizService
{
    Task AddQuizzes();
}

public class AddQuizService : IAddQuizService
{
    
    private readonly QuizRepository _quizRepository;
    private readonly EnvVars _envVars;

    public AddQuizService(QuizRepository quizRepository, EnvVars envVars)
    {
        _quizRepository = quizRepository;
        _envVars = envVars;
    }
    
    private string[] GetFiles()
    {
        string folderPath = @"./Assets/quiz";

        string[] files = Directory.GetFiles(folderPath);

        return files;
    }

    public async Task AddQuizzes()
    {
        var files = GetFiles();
        var existedQuestionType = await _quizRepository.GetQuestionTypeListAsync();
        foreach (var file in files)
        {
            var json = await File.ReadAllTextAsync(file);
            var quizData = JsonSerializer.Deserialize<NewQuiz>(json);
            if (IsQuestionType(existedQuestionType, quizData.quizType))
            {
                Console.WriteLine($"Question type {quizData.quizType} already exists.");
            }
            else
            {
                var questionType = AddQuestionType(quizData.quizType);
                AddQuestionsAnswers(questionType, quizData.questions);
                Console.WriteLine($"Question type {quizData.quizType} added to database.");
            }
            
        }
    }

    private bool IsQuestionType(List<QuestionTypeDto> existedQuestionType, string questionType)
    {
        return existedQuestionType.Any(qt => qt.Description == questionType);
    }

    private QuestionType AddQuestionType(string questionType)
    {
        return _quizRepository.AddQuestionType(questionType);
    }
    
    private void AddQuestionsAnswers(QuestionType questionType, List<NewQuestion> questions)
    {
        foreach (var question in questions)
        {
            var createdQuestion = _quizRepository.AddQuestion(question, questionType);
            foreach (var answer in question.answers)
            {
                _quizRepository.AddAnswer(answer, createdQuestion);
            }
        }
    }
}