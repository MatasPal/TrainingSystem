using System.ComponentModel.DataAnnotations;
using TrainingSystem.Auth.Model;

namespace TrainingSystem.Data.Entities;

public class Comment
{
    public int Id { get; set; }
    
    public required string Text { get; set; }
    
    public int WorkoutId { get; set; }
    public Workout Workout { get; set; }
    
    public int TrainerId { get; set; }
    public Trainer Trainer { get; set; }
    
    [Required]
    public required string UserId { get; set; }
    public ForumUser User { get; set; }
    public CommentDto ToDto()
    {
        return new CommentDto(Id, Text);
    }
}
public record class CommentDto(int Id, string Text);