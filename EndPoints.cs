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
        var commentGroup = app.MapGroup("/api/trainers/{trainerId}/workouts/{workoutId}").AddFluentValidationAutoValidation();
        commentGroup.MapGet("/comments", async (int trainerId, int workoutId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var trainer = await dbContext.Trainers.FindAsync(new object[] { trainerId }, cancellationToken);
            if (trainer == null)
            {
                return Results.NotFound("No trainer found by this ID");
            }

            var workout = await dbContext.Workouts
                .Where(w => w.Id == workoutId && w.TrainerId == trainerId)
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

        commentGroup.MapPost("/comments", [Authorize(Roles = ForumRoles.ForumUser)] async (int trainerId, int workoutId, CreateCommentDto CreateCommentDto, HttpContext httpContext, ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var trainer = await dbContext.Trainers.FindAsync(new object[] { trainerId }, cancellationToken);
            if (trainer == null)
            {
                return Results.NotFound("Trainer not found");
            }

            var workout = await dbContext.Workouts
                .FirstOrDefaultAsync(w => w.Id == workoutId && w.TrainerId == trainerId, cancellationToken);
            if (workout == null)
            {
                return Results.NotFound("Workout not found for this trainer");
            }

            var comment = new Comment()
            {
                Text = CreateCommentDto.Text,
                WorkoutId = workoutId,
                TrainerId = trainerId,
                UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            };
    
            dbContext.Comments.Add(comment);
            await dbContext.SaveChangesAsync(cancellationToken);
    
            return TypedResults.Created($"/api/trainers/{trainerId}/workouts/{workoutId}/comments/{comment.Id}", comment.ToDto());
                
        }).WithName("CreateComment");
        
        commentGroup.MapGet("/comments/{commentId}", async (int trainerId, int workoutId, int commentId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var trainer = await dbContext.Trainers.FindAsync(new object[] { trainerId }, cancellationToken);
            if (trainer == null)
            {
                return Results.NotFound("Trainer not found");
            }

            var workout = await dbContext.Workouts
                .Where(w => w.Id == workoutId && w.TrainerId == trainerId)
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

        commentGroup.MapPut("/comments/{commentId}", [Authorize] async (int trainerId, int workoutId, int commentId, UpdateCommentDto dto, ForumDbContext dbContext, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var trainer = await dbContext.Trainers.FindAsync(new object[] { trainerId }, cancellationToken);
            if (trainer == null)
            {
                return Results.NotFound("Trainer not found");
            }
            var workout = await dbContext.Workouts
                .Where(w => w.Id == workoutId && w.TrainerId == trainerId)
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
        
        commentGroup.MapDelete("/comments/{commentId}", [Authorize] async (int trainerId, int workoutId, int commentId, ForumDbContext dbContext, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var trainer = await dbContext.Trainers.FindAsync(new object[] { trainerId }, cancellationToken);
            if (trainer == null)
            {
                return Results.NotFound("Trainer not found");
            }
            
            var workout = await dbContext.Workouts
                .Where(w => w.Id == workoutId && w.TrainerId == trainerId)
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
        var workoutGroup = app.MapGroup("/api/trainers/{trainerId}").AddFluentValidationAutoValidation();
        
        workoutGroup.MapGet("/workouts", async (int trainerId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var trainer = await dbContext.Trainers.FindAsync(new object[] { trainerId }, cancellationToken);
            if (trainer == null)
            {
                return Results.NotFound("No trainer found by this ID");
            }

            var workout = await dbContext.Workouts
                .Where(w => w.Trainer.Id == trainerId)
                .ToListAsync(cancellationToken);
            return Results.Ok(workout.Select(workout => workout.ToDto()));
        });

        workoutGroup.MapPost("/workouts", [Authorize(Roles = ForumRoles.ForumUser)] async (int trainerId, CreateWorkoutDto createWorkoutDto, HttpContext httpContext, ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var trainer = await dbContext.Trainers.FindAsync(new object[] { trainerId }, cancellationToken);
            if (trainer == null)
            {
                return Results.NotFound("No trainer found by this ID");
            }
            var workout = new Workout()
            {
                TypeTr = createWorkoutDto.TypeTr,
                Place = createWorkoutDto.Place,
                Price = createWorkoutDto.Price,
                TrainerId = trainerId,
                UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            };
    
            dbContext.Workouts.Add(workout);
    
            await dbContext.SaveChangesAsync(cancellationToken);
    
            return TypedResults.Created($"/api/trainers/{trainerId}/workouts/{workout.Id}", workout.ToDto());

        }).WithName("CreateWorkout");
        
        workoutGroup.MapGet("/workouts/{workoutId}", async (int trainerId, int workoutId,  ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var trainer = await dbContext.Trainers.FindAsync(new object[] { trainerId }, cancellationToken);
            if (trainer == null)
            {
                return Results.NotFound("No trainer found by this ID");
            }
            var workout = await dbContext.Workouts
                .Where(w => w.Id == workoutId && w.TrainerId == trainerId)
                .FirstOrDefaultAsync(cancellationToken);
            if (workout == null)
            {
                return Results.NotFound("No workout found by this ID");
            }
            return TypedResults.Ok(workout.ToDto());
        });

        workoutGroup.MapPut("/workouts/{workoutId}", [Authorize] async (int trainerId, int workoutId, UpdateWorkoutDto dto, ForumDbContext dbContext, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var trainer = await dbContext.Trainers.FindAsync(new object[] { trainerId }, cancellationToken);
            if (trainer == null)
            {
                return Results.NotFound("No trainer found by this ID");
            }
            var workout = await dbContext.Workouts
                .Where(w => w.Id == workoutId && w.TrainerId == trainerId)
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
        
        workoutGroup.MapDelete("/workouts/{workoutId}", [Authorize] async (int trainerId, int workoutId, ForumDbContext dbContext, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var trainer = await dbContext.Trainers.FindAsync(new object[] { trainerId }, cancellationToken);
            if (trainer == null)
            {
                return Results.NotFound("No trainer found by this ID");
            }
            var workout = await dbContext.Workouts
                .Where(w => w.Id == workoutId && w.TrainerId == trainerId)
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

    //TRAINER API
    public static void AddTrainerApi(this WebApplication app)
    {
        var trainersGroup = app.MapGroup("/api").AddFluentValidationAutoValidation();

        trainersGroup.MapGet("/trainers", async (ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            return (await dbContext.Trainers.ToListAsync(cancellationToken)).Select(trainer => trainer.ToDto());
        });
        
        trainersGroup.MapPost("/trainers", [Authorize(Roles = ForumRoles.ForumUser)] async (CreateTrainerDto createTrainerDto, HttpContext httpContext, ForumDbContext dbContext) =>
        {
            var trainer = new Trainer()
            {
                Name = createTrainerDto.Name,
                Experience = createTrainerDto.Experience,
                TypeTr = createTrainerDto.TypeTr,
                UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            };
    
            dbContext.Trainers.Add(trainer);
    
            await dbContext.SaveChangesAsync();
    
            return TypedResults.Created($"/api/trainers/{trainer.Id}", trainer.ToDto());
        }).WithName("CreateTrainer");

        trainersGroup.MapGet("/trainers/{trainerId}", async (int trainerId, ForumDbContext dbContext) =>
        {
            var trainer = await dbContext.Trainers.FindAsync(trainerId);
            return trainer == null ? Results.NotFound("No trainer found by this ID") : TypedResults.Ok(trainer.ToDto());
        });
        
        trainersGroup.MapPut("/trainers/{trainerId}", [Authorize] async (UpdateTrainerDto dto, int trainerId, HttpContext httpContext, ForumDbContext dbContext) =>
        {
            var trainer = await dbContext.Trainers.FindAsync(trainerId);
            if (trainer == null)
            {
                return Results.NotFound("No trainer found by this ID");
            }
            
            if (!httpContext.User.IsInRole(ForumRoles.Admin) &&
                httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != trainer.UserId)
            {
                //NotFound()
                return Results.Forbid();
            }
    
            trainer.Experience = dto.Experience;
            trainer.TypeTr = dto.TypeTr;
            
            dbContext.Trainers.Update(trainer);
            await dbContext.SaveChangesAsync();
    
            return Results.Ok(trainer.ToDto());
        });

        trainersGroup.MapDelete("/trainers/{trainerId}", [Authorize] async (int trainerId, HttpContext httpContext, ForumDbContext dbContext) =>
        {
            var trainer = await dbContext.Trainers.FindAsync(trainerId);
            if (trainer == null)
            {
                return Results.NotFound("No trainer found by this ID");
            }
            
            if (!httpContext.User.IsInRole(ForumRoles.Admin) &&
                httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != trainer.UserId)
            {
                //NotFound()
                return Results.Forbid();
            }
            
            dbContext.Trainers.Remove(trainer);
            await dbContext.SaveChangesAsync();
    
            return Results.NoContent();
        });
    }
    
}