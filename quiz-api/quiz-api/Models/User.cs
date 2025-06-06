using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("user")]
public class User
{
    [Key]
    [Column("uuid")]
    public Guid Uuid { get; set; }

    [Required]
    [Column("email")]
    public string Email { get; set; }
    
    [Required]
    [Column("password_hash")]
    public byte[] PasswordHash { get; set; }
    
    [Required]
    [Column("password_salt")]
    public string PasswordSalt { get; set; }
    
    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Column("deleted")]
    public bool Deleted { get; set; } = false;
    
    [Column("date_created")]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow.Date;
    
    [Column("date_updated")]
    public DateTime DateUpdated { get; set; } = DateTime.UtcNow.Date;

    [Column("reset_password_token")]
    public string? ResetPasswordToken { get; set; }
    
    [Column("verify_email_token")]
    public string? VerifyEmailToken { get; set; }

    [Column("user_confirmed")]
    public bool UserConfirmed { get; set; } = false;

    // Navigation properties (if used elsewhere)
    public ICollection<QuizSession> QuizSessions { get; set; }
    public ICollection<QuizTableResult> QuizResults { get; set; }
}