using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
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

        commentGroup.MapPost("/comments", async (int trainerId, int workoutId, CreateCommentDto CreateCommentDto, ForumDbContext dbContext, CancellationToken cancellationToken) =>
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
                TrainerId = trainerId
            };
    
            dbContext.Comments.Add(comment);
            await dbContext.SaveChangesAsync(cancellationToken);
    
            return TypedResults.Created($"/api/trainers/{trainerId}/workouts/{workoutId}/comments/{comment.Id}", comment.ToDto());
                
        });
        /*commentGroup.MapGet("/comments/{commentId}", async (int trainerId, int workoutId, int commentId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
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
                .Where(c => c.Id == commentId && c.WorkoutId == workoutId && c.TrainerId == trainerId)
                .ToListAsync(cancellationToken);
            if (comment == null)
            {
                return Results.NotFound("Comment not found");
            }
            return Results.Ok(comment.Select(comment => comment.ToDto()));
            //return TypedResults.Ok(comment.ToDto());
        });*/
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

        commentGroup.MapPut("/comments/{commentId}", async (int trainerId, int workoutId, int commentId, UpdateCommentDto dto, ForumDbContext dbContext, CancellationToken cancellationToken) =>
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
            
            comment.Text = dto.Text;
    
            dbContext.Comments.Update(comment);
            await dbContext.SaveChangesAsync();
    
            return Results.Ok(comment.ToDto());
                
        });
        
        commentGroup.MapDelete("/comments/{commentId}", async (int trainerId, int workoutId, int commentId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
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

        workoutGroup.MapPost("/workouts", async (int trainerId, CreateWorkoutDto createWorkoutDto, ForumDbContext dbContext, CancellationToken cancellationToken) =>
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
                TrainerId = trainerId
            };
    
            dbContext.Workouts.Add(workout);
    
            await dbContext.SaveChangesAsync(cancellationToken);
    
            return TypedResults.Created($"/api/trainers/{trainerId}/workouts/{workout.Id}", workout.ToDto());

        });
        
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

        workoutGroup.MapPut("/workouts/{workoutId}", async (int trainerId, int workoutId, UpdateWorkoutDto dto, ForumDbContext dbContext, CancellationToken cancellationToken) =>
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
    
            dbContext.Workouts.Update(workout);
            await dbContext.SaveChangesAsync();
    
            return Results.Ok(workout.ToDto());
                
        });
        
        workoutGroup.MapDelete("/workouts/{workoutId}", async (int trainerId, int workoutId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
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
        
        trainersGroup.MapPost("/trainers", async (CreateTrainerDto createTrainerDto, ForumDbContext dbContext) =>
        {
            var trainer = new Trainer()
            {
                Name = createTrainerDto.Name,
                Experience = createTrainerDto.Experience,
                TypeTr = createTrainerDto.TypeTr
            };
    
            dbContext.Trainers.Add(trainer);
    
            await dbContext.SaveChangesAsync();
    
            return TypedResults.Created($"/api/trainers/{trainer.Id}", trainer.ToDto());
        });

        trainersGroup.MapGet("/trainers/{trainerId}", async (int trainerId, ForumDbContext dbContext) =>
        {
            var trainer = await dbContext.Trainers.FindAsync(trainerId);
            return trainer == null ? Results.NotFound("No trainer found by this ID") : TypedResults.Ok(trainer.ToDto());
        });
        
        trainersGroup.MapPut("/trainers/{trainerId}", async(UpdateTrainerDto dto, int trainerId, ForumDbContext dbContext) =>
        {
            var trainer = await dbContext.Trainers.FindAsync(trainerId);
            if (trainer == null)
            {
                return Results.NotFound("No trainer found by this ID");
            }

            trainer.Experience = dto.Experience;
            trainer.TypeTr = dto.TypeTr;
    
            dbContext.Trainers.Update(trainer);
            await dbContext.SaveChangesAsync();
    
            return Results.Ok(trainer.ToDto());
        });

        trainersGroup.MapDelete("/trainers/{trainerId}", async (int trainerId, ForumDbContext dbContext) =>
        {
            var trainer = await dbContext.Trainers.FindAsync(trainerId);
            if (trainer == null)
            {
                return Results.NotFound("No trainer found by this ID");
            }
            dbContext.Trainers.Remove(trainer);
            await dbContext.SaveChangesAsync();
    
            return Results.NoContent();
        });
    }
    
}