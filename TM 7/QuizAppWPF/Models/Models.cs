namespace QuizAppWPF.Models;

public record UserAccount(int UserId, string Username, string FullName, string Role)
{
    public bool IsTeacher => string.Equals(Role, "Teacher", StringComparison.OrdinalIgnoreCase);
    public bool IsStudent => string.Equals(Role, "Student", StringComparison.OrdinalIgnoreCase);
}
public record QuizSummary(int QuizId, string Title, string? Description, int DurationMinutes, int QuestionCount, decimal MaxWeight);
public record AnswerOption(int OptionId, string OptionLabel, string OptionText);
public record QuestionForTaking(int QuestionId, string QuestionText, decimal Weight, string? ImagePath, List<AnswerOption> Options);
public record QuizForTaking(int QuizId, string Title, string? Description, int DurationMinutes, List<QuestionForTaking> Questions);
public record QuizResult(int AttemptId, decimal EarnedWeight, decimal MaxWeight, decimal Percentage);
public record HistoryItem(int AttemptId, int QuizId, string Title, DateTime? FinishedAt, decimal Percentage, decimal EarnedWeight, decimal MaxWeight);
public record ReviewOption(int OptionId, string OptionLabel, string OptionText, bool IsCorrect, bool IsSelected);
public record ReviewQuestion(int QuestionId, string QuestionText, decimal Weight, string? ImagePath, decimal EarnedWeight, List<ReviewOption> Options);
public record AttemptReview(int AttemptId, string Title, decimal Percentage, decimal EarnedWeight, decimal MaxWeight, List<ReviewQuestion> Questions);
public record TeacherQuizSummary(int QuizId, string Title, string? Description, int DurationMinutes, int QuestionCount, decimal MaxWeight, bool IsActive, int AttemptCount);
public record TeacherAnswerRow(int AttemptId, string Username, string FullName, DateTime StartedAt, DateTime? FinishedAt, int QuestionId, string QuestionText, string? SelectedOptionText, string? CorrectOptionText, bool? IsCorrect, decimal AttemptEarnedWeight, decimal AttemptPercentage, decimal AttemptMaxWeight);
