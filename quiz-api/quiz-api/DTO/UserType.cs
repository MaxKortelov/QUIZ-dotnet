using quiz_api.Models;

public class UserDto
{
    public Guid Uuid { get; set; }
    public string Email { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
}

public class LoadUserDto
{
    public required string Email { get; set; }
}

public class UserDataDto
{
    public Guid Uuid { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public int QuizAmountTaken { get; set; }
    public string FastestTestTime { get; set; }
    public int CorrectAnswers { get; set; }
    
    public UserDataDto(User user, QuizTableResult quizTableResult, string fastestTestTime)
    {
        Uuid = user.Uuid;
        Email = user.Email;
        Username = user.Username;
        DateCreated = user.DateCreated;
        DateUpdated = user.DateUpdated;
        QuizAmountTaken = quizTableResult.QuizAmountTaken;
        FastestTestTime = fastestTestTime;
        CorrectAnswers = quizTableResult.CorrectAnswers;
    }
}   