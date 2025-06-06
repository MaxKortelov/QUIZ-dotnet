public class EnvVars
{
    public string Origin { get; set; } = "";

    public EnvVars(IConfiguration configuration)
    {
        Origin = configuration["Origin"];
    }
}