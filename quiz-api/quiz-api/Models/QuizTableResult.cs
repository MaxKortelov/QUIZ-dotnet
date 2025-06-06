using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("quiz_table_results")]
public class QuizTableResult
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; }

    [Column("quiz_amount_taken")]
    public int QuizAmountTaken { get; set; } = 0;
    
    [Column("correct_answers")]
    public int CorrectAnswers { get; set; } = 0;

    [Column("best_quiz_session_id")]
    public Guid? BestQuizSessionId { get; set; }
    
    [ForeignKey("BestQuizSessionId")]
    public QuizSession BestQuizSession { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; }
}