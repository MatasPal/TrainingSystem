using System.ComponentModel.DataAnnotations;
using TrainingSystem.Auth.Model;

namespace TrainingSystem.Data.Entities;

public class Trainer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int Experience { get; set; }
    public required string TypeTr { get; set; }
    public bool IsBlocked{get; set;}
    
    [Required]
    public required string UserId { get; set; }
    public ForumUser User { get; set; }

    public TrainerDto ToDto()
    {
        return new TrainerDto(Id, Name, Experience, TypeTr);
    }
}

public record TrainerDto(int Id, string Name, int Experience, string TypeTr);

public record CreateTrainerDto(int Id, string Name, int Experience, string TypeTr);