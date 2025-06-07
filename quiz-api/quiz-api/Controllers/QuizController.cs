using Microsoft.AspNetCore.Mvc;

namespace quiz_api.Controllers
{
    [Route("quiz/list")]
    [ApiController]
    public class QuizList : ControllerBase
    {
        private readonly IQuizService _quizService;
        
        public QuizList(IQuizService quizService)
        {
            _quizService = quizService;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<QuestionType>>> GetQuizList()
        {
            var types = await _quizService.GetQuizTypesAsync();
            return Ok(new { quizSessions = types });
        }
    }
    
    [Route("quiz/generate")]
    [ApiController]
    public class QuizSessionGenerate(IUserService userService, IQuizService quizService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<QuizSessionRecord>> GenerateQuizSession([FromBody] GenerateQuizSessionDto generateQuizSessionDto)
        {
            var user = await userService.LoadUser(generateQuizSessionDto.Email);
            QuizSessionDto quizSession = await quizService.GenerateQuizSession(generateQuizSessionDto.QuizTypeId, user.Uuid);

            return StatusCode(201, new QuizSessionRecord(quizSession));
        }
    }
    
    [Route("quiz/start")]
    [ApiController]
    public class QuizSessionStart(IUserService userService, IQuizService quizService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<QuizDataDto>> StartQuizSession([FromBody] StartQuizSessionDto startQuizSessionDto)
        {
            var user = await userService.LoadUser(startQuizSessionDto.Email);
            var quizData = await quizService.InitiateQuizSessionAsync(startQuizSessionDto.QuizSessionId, user.Uuid);

            return StatusCode(200, quizData);
        }
    }
    
    [Route("question/save")]
    [ApiController]
    public class QuestionSave : ControllerBase
    {
        [HttpPost]
        public IEnumerable<string> SaveQuestion()
        {
            return new List<string> { "Jan", "Anna", "Piotr" };
        }
    }
    
    [Route("question/next")]
    [ApiController]
    public class QuestionNext : ControllerBase
    {
        [HttpPost]
        public IEnumerable<string> NextQuestion()
        {
            return new List<string> { "Jan", "Anna", "Piotr" };
        }
    }
    
    [Route("submit")]
    [ApiController]
    public class QuizSubmit : ControllerBase
    {
        [HttpPost]
        public IEnumerable<string> SubmitQuiz()
        {
            return new List<string> { "Jan", "Anna", "Piotr" };
        }
    }
}