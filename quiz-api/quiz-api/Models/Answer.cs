using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("answer")]
public class Answer
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; }
    
    [Required]
    [Column("answer_text")]
    public string AnswerText { get; set; }

    [Column("question_id")]
    public Guid? QuestionId { get; set; }
    
    [ForeignKey("QuestionId")]
    public Question Question { get; set; }
}