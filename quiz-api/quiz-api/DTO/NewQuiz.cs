public class NewQuiz
{
    public string quizType { get; set; }
    public List<NewQuestion> questions { get; set; }
}

public class NewAnswer
{
    public string id { get; set; }
    public string text { get; set; }
}

public class NewQuestion
{
    public string question { get; set; }
    public List<NewAnswer> answers { get; set; }
    public string answerId { get; set; }
}