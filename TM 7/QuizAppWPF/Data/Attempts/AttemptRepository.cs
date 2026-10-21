using Microsoft.Data.SqlClient;
using QuizAppWPF.Models;

namespace QuizAppWPF.Data.Attempts;

public sealed class AttemptRepository
{
    private readonly string _connectionString;
    public AttemptRepository(string connectionString) => _connectionString = connectionString;

    public List<HistoryItem> GetHistory(int userId)
    {
        var list = new List<HistoryItem>();
        using var con = new SqlConnection(_connectionString); con.Open();
        using var cmd = new SqlCommand(@"SELECT a.AttemptId,q.QuizId,q.Title,a.FinishedAt,a.FinalPercentage,a.TotalEarnedWeight,
            COALESCE((SELECT SUM(Weight) FROM dbo.Questions WHERE QuizId=q.QuizId),0)
            FROM dbo.UserQuizAttempts a JOIN dbo.Quizzes q ON q.QuizId=a.QuizId
            WHERE a.UserId=@user ORDER BY a.StartedAt DESC", con);
        cmd.Parameters.AddWithValue("@user", userId);
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(new HistoryItem(r.GetInt32(0), r.GetInt32(1), r.GetString(2), r.IsDBNull(3) ? null : r.GetDateTime(3), r.GetDecimal(4), r.GetDecimal(5), r.GetDecimal(6)));
        return list;
    }

    public AttemptReview? GetAttemptReview(int userId, int attemptId)
    {
        using var con = new SqlConnection(_connectionString); con.Open();
        using var attempt = new SqlCommand(@"SELECT q.QuizId,q.Title,a.FinalPercentage,a.TotalEarnedWeight,
            COALESCE((SELECT SUM(Weight) FROM dbo.Questions WHERE QuizId=q.QuizId),0)
            FROM dbo.UserQuizAttempts a JOIN dbo.Quizzes q ON q.QuizId=a.QuizId
            WHERE a.AttemptId=@attempt AND a.UserId=@user AND a.FinishedAt IS NOT NULL", con);
        attempt.Parameters.AddWithValue("@attempt", attemptId); attempt.Parameters.AddWithValue("@user", userId);
        int quizId; string title; decimal percentage, earned, max;
        using (var r = attempt.ExecuteReader())
        {
            if (!r.Read()) return null;
            quizId = r.GetInt32(0); title = r.GetString(1); percentage = r.GetDecimal(2); earned = r.GetDecimal(3); max = r.GetDecimal(4);
        }

        var questions = new List<ReviewQuestion>();
        using var cmd = new SqlCommand(@"SELECT q.QuestionId,q.QuestionText,q.Weight,q.ImagePath,COALESCE(ua.EarnedWeight,0),
            o.OptionId,o.OptionLabel,o.OptionText,o.IsCorrect,CASE WHEN ua.SelectedOptionId=o.OptionId THEN 1 ELSE 0 END
            FROM dbo.Questions q
            LEFT JOIN dbo.UserAnswers ua ON ua.QuestionId=q.QuestionId AND ua.AttemptId=@attempt
            LEFT JOIN dbo.QuestionOptions o ON o.QuestionId=q.QuestionId
            WHERE q.QuizId=@quiz ORDER BY q.OrderNumber,q.QuestionId,o.OptionLabel", con);
        cmd.Parameters.AddWithValue("@attempt", attemptId); cmd.Parameters.AddWithValue("@quiz", quizId);
        using (var r = cmd.ExecuteReader())
        {
            while (r.Read())
            {
                var questionId = r.GetInt32(0);
                if (questions.Count == 0 || questions[^1].QuestionId != questionId)
                    questions.Add(new ReviewQuestion(questionId, r.GetString(1), r.GetDecimal(2), r.IsDBNull(3) ? null : r.GetString(3), r.GetDecimal(4), new()));
                if (!r.IsDBNull(5)) questions[^1].Options.Add(new ReviewOption(r.GetInt32(5), r.GetString(6).Trim(), r.GetString(7), r.GetBoolean(8), r.GetInt32(9) == 1));
            }
        }
        return new AttemptReview(attemptId, title, percentage, earned, max, questions);
    }
}
