namespace admin_blazor.Models;

public class QuizQuestion
{
    public uint Id { get; set; }
    public uint? QuizId { get; set; }
    public string? Question { get; set; }
    public string? Answer1 { get; set; }
    public string? Answer2 { get; set; }
    public string? Answer3 { get; set; }
    public uint? CorrectAnswer { get; set; }
}