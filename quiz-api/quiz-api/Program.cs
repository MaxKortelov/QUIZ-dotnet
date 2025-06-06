using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalhostPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .WithMethods("GET", "POST")
            .AllowAnyHeader(); 
    });
});

// Add PostgresSQL support
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add configs
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        return new BadRequestObjectResult(new ErrorResponse { errors = errors });
    };
});

builder.Services.AddOpenApi();

// Register controllers
builder.Services.AddControllers();
    
// Register validators
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<VerifyEmailDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoadUserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GenerateQuizSessionValidator>();

// Register repositories
builder.Services.AddScoped<QuizRepository>();
builder.Services.AddScoped<UserRepository>();

// Register services
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<CryptoService>();
builder.Services.AddSingleton<DateService>();
builder.Services.AddSingleton<EnvVars>();

var app = builder.Build();

// Add middleware. ErrorHandlingMiddleware must be the first!
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("LocalhostPolicy");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();