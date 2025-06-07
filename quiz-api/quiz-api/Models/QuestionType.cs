using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("question_type")]
public class QuestionType
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; }

    
    [Required]
    [MaxLength(200)]
    [Column("description")]
    public string Description { get; set; }

    public ICollection<QuizSession> QuizSessions { get; set; }
}