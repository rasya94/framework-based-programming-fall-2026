using Microsoft.Data.SqlClient;
using QuizAppWPF.Models;

namespace QuizAppWPF.Data.Teachers;

public sealed class TeacherRepository
{
    private readonly string _connectionString;
    public TeacherRepository(string connectionString) => _connectionString = connectionString;

    public List<TeacherQuizSummary> GetAllQuizzes()
    {
        var list = new List<TeacherQuizSummary>();
        using var con = new SqlConnection(_connectionString); con.Open();
        using var cmd = new SqlCommand(@"SELECT q.QuizId,q.Title,q.Description,q.DurationMinutes,
            (SELECT COUNT(*) FROM dbo.Questions qu WHERE qu.QuizId=q.QuizId),
            COALESCE((SELECT SUM(qu.Weight) FROM dbo.Questions qu WHERE qu.QuizId=q.QuizId),0),q.IsActive,
            (SELECT COUNT(*) FROM dbo.UserQuizAttempts a WHERE a.QuizId=q.QuizId)
            FROM dbo.Quizzes q ORDER BY q.CreatedAt DESC", con);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(new TeacherQuizSummary(r.GetInt32(0), r.GetString(1), r.IsDBNull(2) ? null : r.GetString(2), r.GetInt32(3), r.GetInt32(4), r.GetDecimal(5), r.GetBoolean(6), r.GetInt32(7)));
        return list;
    }

    public List<TeacherAnswerRow> GetQuizAnswers(int quizId)
    {
        var list = new List<TeacherAnswerRow>();
        using var con = new SqlConnection(_connectionString); con.Open();
        using var cmd = new SqlCommand(@"SELECT a.AttemptId,u.Username,u.FullName,a.StartedAt,a.FinishedAt,
            q.QuestionId,q.QuestionText,selected.OptionText,correct.OptionText,selected.IsCorrect,
            a.TotalEarnedWeight,a.FinalPercentage,COALESCE((SELECT SUM(weightedQuestion.Weight) FROM dbo.Questions weightedQuestion WHERE weightedQuestion.QuizId=a.QuizId),0)
            FROM dbo.UserQuizAttempts a
            JOIN dbo.Users u ON u.UserId=a.UserId
            JOIN dbo.Questions q ON q.QuizId=a.QuizId
            LEFT JOIN dbo.UserAnswers ua ON ua.AttemptId=a.AttemptId AND ua.QuestionId=q.QuestionId
            LEFT JOIN dbo.QuestionOptions selected ON selected.OptionId=ua.SelectedOptionId
            OUTER APPLY (SELECT TOP (1) o.OptionText FROM dbo.QuestionOptions o WHERE o.QuestionId=q.QuestionId AND o.IsCorrect=1 ORDER BY o.OptionLabel) correct
            WHERE a.QuizId=@quiz ORDER BY a.StartedAt DESC,q.OrderNumber,q.QuestionId", con);
        cmd.Parameters.AddWithValue("@quiz", quizId);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(new TeacherAnswerRow(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetDateTime(3), r.IsDBNull(4) ? null : r.GetDateTime(4),
                r.GetInt32(5), r.GetString(6), r.IsDBNull(7) ? null : r.GetString(7), r.IsDBNull(8) ? null : r.GetString(8), r.IsDBNull(9) ? null : r.GetBoolean(9), r.GetDecimal(10), r.GetDecimal(11), r.GetDecimal(12)));
        return list;
    }
}
