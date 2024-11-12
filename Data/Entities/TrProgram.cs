using System.ComponentModel.DataAnnotations;
using TrainingSystem.Auth.Model;

namespace TrainingSystem.Data.Entities;

public class TrProgram
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Descr { get; set; }
    public required int Difficulty { get; set; }
    public string Trainer {get; set;}
    public string Duration {get; set;}
    
    public bool IsBlocked{get; set;}
    
    [Required]
    public required string UserId { get; set; }
    public ForumUser User { get; set; }

    public TrProgramDto ToDto()
    {
        return new TrProgramDto(Id, Name, Descr, Difficulty, Trainer, Duration);
    }
}

public record TrProgramDto(int Id, string Name, string Descr, int Difficulty, string Trainer, string Duration);

public record CreateTrProgramDto(int Id, string Name, string Descr, int Difficulty, string Trainer, string Duration);