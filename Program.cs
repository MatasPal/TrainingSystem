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

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ForumDbContext>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.OverrideDefaultResultFactoryWith<ProblemDetailsResultFactory>();
});
var app = builder.Build();

/*
/api/v1/topics GET List 200
/api/v1/topics{id} GET One 200
/api/v1/topics POST Create 201
/api/v1/topics{id} PUT/PATCH Modify 200
/api/v1/topics{id} DELETE Remove 200/204
*/

app.AddTrainerApi();
app.AddWorkoutApi();
app.AddCommentApi();


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
//TRAINER DTO
public record CreateTrainerDto(string Name, int Experience, string TypeTr)
{
    public class CreateTrainerDtoValidator : AbstractValidator<CreateTrainerDto>
    {
        public CreateTrainerDtoValidator()
        {
            RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(2, 100);
            RuleFor(dto => dto.Experience).NotEmpty().NotNull();
            RuleFor(dto => dto.TypeTr).NotEmpty().NotNull().Length(2, 100);
        }
    }
};
public record UpdateTrainerDto(int Experience, string TypeTr)
{
    public class UpdateTrainerDtoValidator : AbstractValidator<CreateTrainerDto>
    {
        public UpdateTrainerDtoValidator()
        {
            RuleFor(dto => dto.Experience).NotEmpty().NotNull();
            RuleFor(dto => dto.TypeTr).NotEmpty().NotNull().Length(2, 100);
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
            RuleFor(dto => dto.Text).NotEmpty().NotNull().Length(2, 100);
        }
    }
}