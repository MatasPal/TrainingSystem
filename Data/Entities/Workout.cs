using System.ComponentModel.DataAnnotations;
using TrainingSystem.Auth.Model;

namespace TrainingSystem.Data.Entities;

public class Workout
{
    public int Id { get; set; }
    public required string TypeTr { get; set; }
    public required string Place { get; set; }
    public required int Price { get; set; }
    public int TrainerId { get; set; }
    public Trainer Trainer { get; set; }
    
    [Required]
    public required string UserId { get; set; }
    public ForumUser User { get; set; }
    public WorkoutDto ToDto()
    {
        return new WorkoutDto(Id, TypeTr, Place, Price);
    }
}

public record WorkoutDto(int Id, string TypeTr, string Place, int Price);

