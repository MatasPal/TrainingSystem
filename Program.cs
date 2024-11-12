using System.Text;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Results;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;
using TrainingSystem;
using TrainingSystem.Data;
using TrainingSystem.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TrainingSystem.Auth;
using TrainingSystem.Auth.Model;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<ForumDbContext>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.OverrideDefaultResultFactoryWith<ProblemDetailsResultFactory>();
});
builder.Services.AddResponseCaching();
builder.Services.AddTransient<JwtTokenService>();
builder.Services.AddScoped<AuthSeeder>();
builder.Services.AddTransient<SessionService>();

//AUTH
builder.Services.AddIdentity<ForumUser, IdentityRole>()
    .AddEntityFrameworkStores<ForumDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters.ValidAudience = builder.Configuration["JWT:ValidAudience"];
    options.TokenValidationParameters.ValidIssuer = builder.Configuration["JWT:ValidIssuer"];
    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]));
});
 
builder.Services.AddAuthorization();

var app = builder.Build();

using var scope = app.Services.CreateScope();

//var dbContext = scope.ServiceProvider.GetRequiredService<ForumDbContext>();

var dbSeeder = scope.ServiceProvider.GetRequiredService<AuthSeeder>();
await dbSeeder.SeedAsync();

/*
/api/v1/topics GET List 200
/api/v1/topics{id} GET One 200
/api/v1/topics POST Create 201
/api/v1/topics{id} PUT/PATCH Modify 200
/api/v1/topics{id} DELETE Remove 200/204
*/

app.AddTrProgramApi();
app.AddWorkoutApi();
app.AddCommentApi();
app.AddAuthApi();

app.MapControllers();
app.UseResponseCaching();
app.UseAuthentication();
app.UseAuthorization();
app.Run();

public class ProblemDetailsResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IResult CreateResult(EndpointFilterInvocationContext context, ValidationResult validationResult)
    {
        var problemDetails = new HttpValidationProblemDetails(validationResult.ToValidationProblemErrors())
        {
            Type = "https:://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "Unprocessable Entity",
            Status = 422
        };
        return TypedResults.Problem(problemDetails);
    }
}
//TRAINING PROGRAM DTO
public record CreateTrProgramDto(string Name, string Descr, int Difficulty, string Trainer, string Duration)
{
    public class CreateTrProgramDtoValidator : AbstractValidator<CreateTrProgramDto>
    {
        public CreateTrProgramDtoValidator()
        {
            RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(3, 30);
            RuleFor(dto => dto.Descr).NotEmpty().NotNull().Length(3, 100);
            RuleFor(dto => dto.Difficulty).NotEmpty().NotNull();
        }
    }
};
public record UpdateTrProgramDto(string Name, string Descr, int Difficulty, string Trainer, string Duration)
{
    public class UpdateTrProgramDtoValidator : AbstractValidator<CreateTrProgramDto>
    {
        public UpdateTrProgramDtoValidator()
        {
            RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(3, 30);
            RuleFor(dto => dto.Descr).NotEmpty().NotNull().Length(3, 100);
            RuleFor(dto => dto.Difficulty).NotEmpty().NotNull();
        }
    }
}

//WORKOUT DTO
public record CreateWorkoutDto(string TypeTr, string Place, int Price)
{
    public class CreateWorkoutDtoValidator : AbstractValidator<CreateWorkoutDto>
    {
        public CreateWorkoutDtoValidator()
        {
            RuleFor(dto => dto.TypeTr).NotEmpty().NotNull().Length(2, 100);
            RuleFor(dto => dto.Place).NotEmpty().NotNull().Length(2, 100);
            RuleFor(dto => dto.Price).NotEmpty().NotNull();
        }
    }
};
public record UpdateWorkoutDto(string TypeTr, string Place, int Price)
{
    public class UpdateWorkoutDtoValidator : AbstractValidator<CreateWorkoutDto>
    {
        public UpdateWorkoutDtoValidator()
        {
            RuleFor(dto => dto.TypeTr).NotEmpty().NotNull().Length(2, 100);
            RuleFor(dto => dto.Place).NotEmpty().NotNull().Length(2, 100);
            RuleFor(dto => dto.Price).NotEmpty().NotNull();
        }
    }
}

//COMMENT DTO
public record CreateCommentDto(string Text)
{
    public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
    {
        public CreateCommentDtoValidator()
        {
            RuleFor(dto => dto.Text).NotEmpty().NotNull().Length(2, 100);
        }
    }
};
public record UpdateCommentDto(string Text)
{
    public class UpdateCommentDtoValidator : AbstractValidator<CreateCommentDto>
    {
        public UpdateCommentDtoValidator()
        {
            RuleFor(dto => dto.Text).NotEmpty().NotNull().Length(min: 2, max: 100);
        }
    }
}


