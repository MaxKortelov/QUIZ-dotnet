public class EnvVars
{
    public string Origin { get; set; } = "";
    public int TimePerQuestion { get; set; } = 2;
    public int AttemptsPerQuiz { get; set; } = 10;

    public EnvVars(IConfiguration configuration)
    {
        Origin = configuration["Origin"];
        TimePerQuestion = int.Parse(configuration["TimePerQuestion"]);
        AttemptsPerQuiz = int.Parse(configuration["AttemptsPerQuiz"]);
    }
}