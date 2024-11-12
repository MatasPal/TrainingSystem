namespace TrainingSystem.Auth.Model;

public class ForumRoles
{
    public const string Admin = nameof(Admin);
    public const string Trainer = nameof(Trainer);
    public const string Athlete = nameof(Athlete);
    
    public static readonly IReadOnlyCollection<string> All = new[] { Admin, Trainer, Athlete };
}