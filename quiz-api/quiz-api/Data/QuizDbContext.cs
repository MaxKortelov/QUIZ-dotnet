using Microsoft.EntityFrameworkCore;

public class QuizDbContext : DbContext
{
    public QuizDbContext(DbContextOptions<QuizDbContext> options)
        : base(options) { }

    // DbSet properties
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<QuizSession> QuizSessions { get; set; }
    public DbSet<QuizTableResult> QuizTableResults { get; set; }
    public DbSet<QuestionType> QuestionTypes { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Table name mappings if needed
        modelBuilder.Entity<User>().ToTable("user");
        modelBuilder.Entity<Question>().ToTable("question");
        modelBuilder.Entity<Answer>().ToTable("answer");
        modelBuilder.Entity<QuizSession>().ToTable("quiz_session");
        modelBuilder.Entity<QuizTableResult>().ToTable("quiz_table_results");
        modelBuilder.Entity<QuestionType>().ToTable("question_type");

        // Property naming for PostgreSQL snake_case if needed
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Uuid);
            entity.Property(u => u.Email).HasColumnName("email");
            entity.Property(u => u.PasswordHash).HasColumnName("password_hash");
            entity.Property(u => u.PasswordSalt).HasColumnName("password_salt");
            entity.Property(u => u.Username).HasColumnName("username");
            entity.Property(u => u.Deleted).HasColumnName("deleted");
            entity.Property(u => u.DateCreated).HasColumnName("date_created");
            entity.Property(u => u.DateUpdated).HasColumnName("date_updated");
            entity.Property(u => u.ResetPasswordToken).HasColumnName("reset_password_token");
            entity.Property(u => u.VerifyEmailToken).HasColumnName("verify_email_token");
            entity.Property(u => u.UserConfirmed).HasColumnName("user_confirmed");
        });

        // Configure other relationships with Fluent API as needed
        modelBuilder.Entity<Question>()
            .HasOne(q => q.QuestionType)
            .WithMany(t => t.Questions)
            .HasForeignKey(q => q.QuestionTypeId);

        modelBuilder.Entity<Answer>()
            .HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId);

        modelBuilder.Entity<QuizSession>()
            .HasOne(s => s.User)
            .WithMany(u => u.QuizSessions)
            .HasForeignKey(s => s.UserId);

        modelBuilder.Entity<QuizSession>()
            .HasOne(s => s.QuestionType)
            .WithMany(t => t.QuizSessions)
            .HasForeignKey(s => s.QuestionTypeId);
        
        modelBuilder.Entity<QuizSession>()
            .Property(q => q.QuestionAnswer)
            .HasColumnType("jsonb");

        modelBuilder.Entity<QuizTableResult>()
            .HasOne(r => r.User)
            .WithMany(u => u.QuizResults)
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<QuizTableResult>()
            .HasOne(r => r.BestQuizSession)
            .WithMany()
            .HasForeignKey(r => r.BestQuizSessionId);
    }
}
