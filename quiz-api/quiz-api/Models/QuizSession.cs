using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("quiz_session")]
public class QuizSession
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; }

    [Column("question_sequence")]
    public List<Guid> QuestionSequence { get; set; }

    [Column("question_answer", TypeName = "jsonb")]
    public string QuestionAnswer { get; set; } = "{}";

    [Column("date_created")]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    
    [Column("date_started")]
    public DateTime? DateStarted { get; set; }
    
    [Column("date_ended")]
    public DateTime? DateEnded { get; set; }

    [Column("duration")]
    public int Duration { get; set; } = 30;
    
    [Column("attempts")]
    public int Attempts { get; set; } = 10;
    
    [Column("attempts_used")]
    public int AttemptsUsed { get; set; } = 0;
    
    [Column("result")]
    public int? Result { get; set; }

    [Column("question_type_id")]
    public Guid? QuestionTypeId { get; set; }
    
    [ForeignKey("QuestionTypeId")]
    public QuestionType QuestionType { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; }
}