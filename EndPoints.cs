using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using TrainingSystem.Auth.Model;
using TrainingSystem.Data;
using TrainingSystem.Data.Entities;

namespace TrainingSystem;

public static class EndPoints
{
    //COMMENT API
    public static void AddCommentApi(this WebApplication app)
    {
        var commentGroup = app.MapGroup("/api/trPrograms/{trProgramId}/workouts/{workoutId}").AddFluentValidationAutoValidation();
        commentGroup.MapGet("/comments", async (int trProgramId, int workoutId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var trProgram = await dbContext.TrPrograms.FindAsync(new object[] { trProgramId }, cancellationToken);
            if (trProgram == null)
            {
                return Results.NotFound("No training program found by this ID");
            }

            var workout = await dbContext.Workouts
                .Where(w => w.Id == workoutId && w.TrProgramId == trProgramId)
                .ToListAsync(cancellationToken);
            if (workout == null)
            {
                return Results.NotFound("No workout found by this ID");
            }
            var comment = await dbContext.Comments
                .Where(c => c.Workout.Id == workoutId)
                .ToListAsync(cancellationToken);
            return Results.Ok(comment.Select(comment => comment.ToDto()));
        });

        commentGroup.MapPost("/comments", [Authorize(Roles = ForumRoles.Athlete)] async (int trProgramId, int workoutId, CreateCommentDto CreateCommentDto, HttpContext httpContext, ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var trProgram = await dbContext.TrPrograms.FindAsync(new object[] { trProgramId }, cancellationToken);
            if (trProgram == null)
            {
                return Results.NotFound("Training program not found");
            }

            var workout = await dbContext.Workouts
                .FirstOrDefaultAsync(w => w.Id == workoutId && w.TrProgramId == trProgramId, cancellationToken);
            if (workout == null)
            {
                return Results.NotFound("Workout not found for this training program");
            }

            var comment = new Comment()
            {
                Text = CreateCommentDto.Text,
                WorkoutId = workoutId,
                TrProgramId = trProgramId,
                UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            };
    
            dbContext.Comments.Add(comment);
            await dbContext.SaveChangesAsync(cancellationToken);
    
            return TypedResults.Created($"/api/trPrograms/{trProgramId}/workouts/{workoutId}/comments/{comment.Id}", comment.ToDto());
                
        }).WithName("CreateComment");
        
        commentGroup.MapGet("/comments/{commentId}", async (int trProgramId, int workoutId, int commentId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var trProgram = await dbContext.TrPrograms.FindAsync(new object[] { trProgramId }, cancellationToken);
            if (trProgram == null)
            {
                return Results.NotFound("Training program not found");
            }

            var workout = await dbContext.Workouts
                .Where(w => w.Id == workoutId && w.TrProgramId == trProgramId)
                .FirstOrDefaultAsync(cancellationToken);
            if (workout == null)
            {
                return Results.NotFound("Workout not found");
            }
            
            var comment = await dbContext.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.WorkoutId == workoutId, cancellationToken); 
        
            if (comment == null)
            {
                return Results.NotFound("Comment not found");
            }
    
            return TypedResults.Ok(comment.ToDto()); 
        });

        commentGroup.MapPut("/comments/{commentId}", [Authorize] async (int trProgramId, int workoutId, int commentId, UpdateCommentDto dto, ForumDbContext dbContext, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var trProgram = await dbContext.TrPrograms.FindAsync(new object[] { trProgramId }, cancellationToken);
            if (trProgram == null)
            {
                return Results.NotFound("Training program not found");
            }
            var workout = await dbContext.Workouts
                .Where(w => w.Id == workoutId && w.TrProgramId == trProgramId)
                .FirstOrDefaultAsync(cancellationToken);
            if (workout == null)
            {
                return Results.NotFound("Workout not found");
            }
            var comment = await dbContext.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.WorkoutId == workoutId, cancellationToken); 
        
            if (comment == null)
            {
                return Results.NotFound("Comment not found");
            }
            
            if (!httpContext.User.IsInRole(ForumRoles.Admin) &&
                httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != workout.UserId)
            {
                //NotFound()
                return Results.Forbid();
            }
            
            comment.Text = dto.Text;
    
            dbContext.Comments.Update(comment);
            await dbContext.SaveChangesAsync();
    
            return Results.Ok(comment.ToDto());
                
        });
        
        commentGroup.MapDelete("/comments/{commentId}", [Authorize] async (int trProgramId, int workoutId, int commentId, ForumDbContext dbContext, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var trProgram = await dbContext.TrPrograms.FindAsync(new object[] { trProgramId }, cancellationToken);
            if (trProgram == null)
            {
                return Results.NotFound("Training program not found");
            }
            
            var workout = await dbContext.Workouts
                .Where(w => w.Id == workoutId && w.TrProgramId == trProgramId)
                .FirstOrDefaultAsync(cancellationToken);
            if (workout == null)
            {
                return Results.NotFound("Workout not found");
            }
            
            var comment = await dbContext.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.WorkoutId == workoutId, cancellationToken); 
            if (comment == null)
            {
                return Results.NotFound("Comment not found");
            }
            
            if (!httpContext.User.IsInRole(ForumRoles.Admin) &&
                httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != workout.UserId)
            {
                //NotFound()
                return Results.Forbid();
            }
            
            dbContext.Comments.Remove(comment);
            await dbContext.SaveChangesAsync();
    
            return Results.NoContent();
        });
        
    }

    //WORKOUT API
    public static void AddWorkoutApi(this WebApplication app)
    {
        var workoutGroup = app.MapGroup("/api/trPrograms/{trProgramId}").AddFluentValidationAutoValidation();
        
        workoutGroup.MapGet("/workouts", async (int trProgramId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var trProgram = await dbContext.TrPrograms.FindAsync(new object[] { trProgramId }, cancellationToken);
            if (trProgram == null)
            {
                return Results.NotFound("No training program found by this ID");
            }

            var workout = await dbContext.Workouts
                .Where(w => w.TrProgram.Id == trProgramId)
                .ToListAsync(cancellationToken);
            return Results.Ok(workout.Select(workout => workout.ToDto()));
        });

        workoutGroup.MapPost("/workouts", [Authorize(Roles = ForumRoles.Trainer)] async (int trProgramId, CreateWorkoutDto createWorkoutDto, HttpContext httpContext, ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var trProgram = await dbContext.TrPrograms.FindAsync(new object[] { trProgramId }, cancellationToken);
            if (trProgram == null)
            {
                return Results.NotFound("No training program found by this ID");
            }
            var workout = new Workout()
            {
                TypeTr = createWorkoutDto.TypeTr,
                Place = createWorkoutDto.Place,
                Price = createWorkoutDto.Price,
                TrProgramId = trProgramId,
                UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            };
            
            if (!httpContext.User.IsInRole(ForumRoles.Admin) &&
                httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != workout.UserId)
            {
                //NotFound()
                return Results.Forbid();
            }
    
            dbContext.Workouts.Add(workout);
    
            await dbContext.SaveChangesAsync(cancellationToken);
    
            return TypedResults.Created($"/api/trPrograms/{trProgramId}/workouts/{workout.Id}", workout.ToDto());

        }).WithName("CreateWorkout");
        
        workoutGroup.MapGet("/workouts/{workoutId}", async (int trProgramId, int workoutId,  ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var trProgram = await dbContext.TrPrograms.FindAsync(new object[] { trProgramId }, cancellationToken);
            if (trProgram == null)
            {
                return Results.NotFound("No training program found by this ID");
            }
            var workout = await dbContext.Workouts
                .Where(w => w.Id == workoutId && w.TrProgramId == trProgramId)
                .FirstOrDefaultAsync(cancellationToken);
            if (workout == null)
            {
                return Results.NotFound("No workout found by this ID");
            }
            return TypedResults.Ok(workout.ToDto());
        });

        workoutGroup.MapPut("/workouts/{workoutId}", [Authorize] async (int trProgramId, int workoutId, UpdateWorkoutDto dto, ForumDbContext dbContext, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var trProgram = await dbContext.TrPrograms.FindAsync(new object[] { trProgramId }, cancellationToken);
            if (trProgram == null)
            {
                return Results.NotFound("No training program found by this ID");
            }
            var workout = await dbContext.Workouts
                .Where(w => w.Id == workoutId && w.TrProgramId == trProgramId)
                .FirstOrDefaultAsync(cancellationToken);
            if (workout == null)
            {
                return Results.NotFound("No workout found by this ID");
            }
            workout.TypeTr = dto.TypeTr;
            workout.Place = dto.Place;
            workout.Price = dto.Price;

            if (!httpContext.User.IsInRole(ForumRoles.Admin) &&
                httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != workout.UserId)
            {
                //NotFound()
                return Results.Forbid();
            }
    
            dbContext.Workouts.Update(workout);
            await dbContext.SaveChangesAsync();
    
            return Results.Ok(workout.ToDto());
                
        });
        
        workoutGroup.MapDelete("/workouts/{workoutId}", [Authorize] async (int trProgramId, int workoutId, ForumDbContext dbContext, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var trProgram = await dbContext.TrPrograms.FindAsync(new object[] { trProgramId }, cancellationToken);
            if (trProgram == null)
            {
                return Results.NotFound("No training program found by this ID");
            }
            var workout = await dbContext.Workouts
                .Where(w => w.Id == workoutId && w.TrProgramId == trProgramId)
                .FirstOrDefaultAsync(cancellationToken);
            if (workout == null)
            {
                return Results.NotFound("No workout found by this ID");
            }
            
            if (!httpContext.User.IsInRole(ForumRoles.Admin) &&
                httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != workout.UserId)
            {
                //NotFound()
                return Results.Forbid();
            }
            
            dbContext.Workouts.Remove(workout);
            await dbContext.SaveChangesAsync();
    
            return Results.NoContent();
        });
    }

    //TRAINING PROGRAM API
    public static void AddTrProgramApi(this WebApplication app)
    {
        var trProgramsGroup = app.MapGroup("/api").AddFluentValidationAutoValidation();

        trProgramsGroup.MapGet("/trPrograms", async (ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            return (await dbContext.TrPrograms.ToListAsync(cancellationToken)).Select(trProgram => trProgram.ToDto());
        });
        
        trProgramsGroup.MapPost("/trPrograms", [Authorize(Roles = ForumRoles.Trainer)] async (CreateTrProgramDto createTrProgramDto, HttpContext httpContext, ForumDbContext dbContext) =>
        {
            var trProgram = new TrProgram()
            {
                Name = createTrProgramDto.Name,
                Descr = createTrProgramDto.Descr,
                Difficulty = createTrProgramDto.Difficulty,
                Trainer = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub),
                Duration = createTrProgramDto.Duration,
                UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            };
    
            dbContext.TrPrograms.Add(trProgram);
    
            await dbContext.SaveChangesAsync();
    
            return TypedResults.Created($"/api/trPrograms/{trProgram.Id}", trProgram.ToDto());
        }).WithName("CreateTrainingProgram");

        trProgramsGroup.MapGet("/trPrograms/{trProgramId}", async (int trProgramId, ForumDbContext dbContext) =>
        {
            var trProgram = await dbContext.TrPrograms.FindAsync(trProgramId);
            return trProgram == null ? Results.NotFound("No training program found by this ID") : TypedResults.Ok(trProgram.ToDto());
        });
        
        trProgramsGroup.MapPut("/trPrograms/{trProgramId}", [Authorize] async (UpdateTrProgramDto dto, int trProgramId, HttpContext httpContext, ForumDbContext dbContext) =>
        {
            var trProgram = await dbContext.TrPrograms.FindAsync(trProgramId);
            if (trProgram == null)
            {
                return Results.NotFound("No training program found by this ID");
            }
            
            if (!httpContext.User.IsInRole(ForumRoles.Admin) &&
                httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != trProgram.UserId)
            {
                //NotFound()
                return Results.Forbid();
            }
    
            trProgram.Name = dto.Name;
            trProgram.Descr = dto.Descr;
            trProgram.Difficulty = dto.Difficulty;
            trProgram.Duration = dto.Duration;
            
            dbContext.TrPrograms.Update(trProgram);
            await dbContext.SaveChangesAsync();
    
            return Results.Ok(trProgram.ToDto());
        }).WithName("UpdateTrainingProgram");

        trProgramsGroup.MapDelete("/trPrograms/{trProgramId}", [Authorize] async (int trProgramId, HttpContext httpContext, ForumDbContext dbContext) =>
        {
            var trProgram = await dbContext.TrPrograms.FindAsync(trProgramId);
            if (trProgram == null)
            {
                return Results.NotFound("No training program found by this ID");
            }
            
            if (!httpContext.User.IsInRole(ForumRoles.Admin) &&
                httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != trProgram.UserId)
            {
                //NotFound()
                return Results.Forbid();
            }
            
            dbContext.TrPrograms.Remove(trProgram);
            await dbContext.SaveChangesAsync();
    
            return Results.NoContent();
        });
    }
    
}