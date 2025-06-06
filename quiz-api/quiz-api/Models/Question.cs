using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("question")]
public class Question
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; }
    
    [Required]
    [Column("question_text")]
    public string QuestionText { get; set; }
    
    [Column("correct_answers")]
    public List<string> CorrectAnswers { get; set; }

    [Column("question_type_id")]
    public Guid? QuestionTypeId { get; set; }
    
    [ForeignKey("QuestionTypeId")]
    public QuestionType QuestionType { get; set; }

    public ICollection<Answer> Answers { get; set; }
}