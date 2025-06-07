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
    
    [Route("quiz/question/save")]
    [ApiController]
    public class QuestionSave(IUserService userService, IQuizService quizService) : ControllerBase
    {
        [HttpPost]
        public async Task<ResponseSuccess> SaveQuestion([FromBody] SaveQuizQuestionDto saveQuizSessionDto)
        {
            var user = await userService.LoadUser(saveQuizSessionDto.Email);
            
            await quizService.AddQuizQuestionAnswerAsync(saveQuizSessionDto, user.Uuid);

            return new ResponseSuccess("Answer is saved successfully");
        }
    }
    
    [Route("quiz/question/next")]
    [ApiController]
    public class QuestionNext(IUserService userService, IQuizService quizService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<QuizDataDto>> NextQuestion([FromBody] SaveQuizQuestionDto saveQuizSessionDto)
        {
            var user = await userService.LoadUser(saveQuizSessionDto.Email);
            
            await quizService.AddQuizQuestionAnswerAsync(saveQuizSessionDto, user.Uuid);

            var question = await quizService.FindNextQuizQuestionAsync(saveQuizSessionDto.QuizSessionId, user.Uuid);
            var quizSession =  await quizService.GetQuizSessionAsync(saveQuizSessionDto, user.Uuid);
            
            return StatusCode(200, new QuizDataDto(question, quizSession));
        }
    }
    
    [Route("quiz/submit")]
    [ApiController]
    public class QuizSubmit(IUserService userService, IQuizService quizService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<SubmitQuizSessionResultResponseDto>> SubmitQuiz([FromBody] SubmitQuizSessionDto submitQuizSessionDto)
        {
            var user = await userService.LoadUser(submitQuizSessionDto.Email);
            var result = await quizService.SubmitQuiz(submitQuizSessionDto, user);

            return StatusCode(200, result);
        }
    }
}