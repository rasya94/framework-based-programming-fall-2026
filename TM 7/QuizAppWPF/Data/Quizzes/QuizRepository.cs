using System.Data;
using Microsoft.Data.SqlClient;
using QuizAppWPF.Models;

namespace QuizAppWPF.Data.Quizzes;

public sealed class QuizRepository 
{
    private readonly string _connectionString;
    public QuizRepository(string connectionString) => _connectionString = connectionString;
    private SqlConnection OpenConnection() => new(_connectionString);

    public List<QuizSummary> GetActiveQuizzes()
    {
        var list = new List<QuizSummary>(); 
        using var con = OpenConnection(); 
        con.Open();

        using var cmd = new SqlCommand(@"
            SELECT 
                q.QuizId, 
                q.Title, 
                q.Description, 
                q.DurationMinutes, 
                COUNT(DISTINCT qu.QuestionId), 
                COALESCE(SUM(qu.Weight), 0) 
            FROM dbo.Quizzes q 
            LEFT JOIN dbo.Questions qu ON qu.QuizId = q.QuizId 
            WHERE q.IsActive = 1 
            GROUP BY q.QuizId, q.Title, q.Description, q.DurationMinutes, q.CreatedAt 
            ORDER BY q.CreatedAt DESC", con);

        using var r = cmd.ExecuteReader();
        while (r.Read()) 
            list.Add(new QuizSummary(r.GetInt32(0), r.GetString(1), r.IsDBNull(2) ? null : r.GetString(2), r.GetInt32(3), r.GetInt32(4), r.GetDecimal(5)));
        
        return list;
    }

    public HashSet<int> GetCompletedQuizIds(int userId)
    {
        var ids = new HashSet<int>();
        using var con = OpenConnection(); 
        con.Open();

        using var cmd = new SqlCommand(@"
            SELECT DISTINCT QuizId 
            FROM dbo.UserQuizAttempts 
            WHERE UserId = @user AND FinishedAt IS NOT NULL", con);

        cmd.Parameters.AddWithValue("@user", userId);

        using var r = cmd.ExecuteReader();
        while (r.Read()) 
            ids.Add(r.GetInt32(0));
        
        return ids;
    }

    public QuizForTaking? StartQuiz(int userId, int quizId)
    {
        using var con = OpenConnection(); 
        con.Open();

        using (var completed = new SqlCommand(@"
            SELECT COUNT(*) 
            FROM dbo.UserQuizAttempts 
            WHERE UserId = @user AND QuizId = @quiz AND FinishedAt IS NOT NULL", con))
        {
            completed.Parameters.AddWithValue("@user", userId); 
            completed.Parameters.AddWithValue("@quiz", quizId);
            if ((int)completed.ExecuteScalar()! > 0) 
                throw new InvalidOperationException("Kuis ini sudah pernah diselesaikan dan tidak dapat diulang.");
        }

        using var check = new SqlCommand(@"
            SELECT Title, Description, DurationMinutes 
            FROM dbo.Quizzes 
            WHERE QuizId = @id AND IsActive = 1", con);
        
        check.Parameters.AddWithValue("@id", quizId);
        string title; string? description; int duration;
        
        using (var r = check.ExecuteReader()) 
        { 
            if (!r.Read()) return null; 
            title = r.GetString(0); 
            description = r.IsDBNull(1) ? null : r.GetString(1); 
            duration = r.GetInt32(2); 
        }

        using var insert = new SqlCommand(@"
            INSERT INTO dbo.UserQuizAttempts(UserId, QuizId) 
            OUTPUT INSERTED.AttemptId 
            VALUES(@user, @quiz)", con);

        insert.Parameters.AddWithValue("@user", userId); 
        insert.Parameters.AddWithValue("@quiz", quizId); 
        var attemptId = (int)insert.ExecuteScalar()!;

        var questions = new List<QuestionForTaking>();
        using var cmd = new SqlCommand(@"
            SELECT QuestionId, QuestionText, Weight, ImagePath 
            FROM dbo.Questions 
            WHERE QuizId = @id 
            ORDER BY OrderNumber, QuestionId", con);

        cmd.Parameters.AddWithValue("@id", quizId);

        using (var r = cmd.ExecuteReader()) 
            while (r.Read()) 
                questions.Add(new QuestionForTaking(r.GetInt32(0), r.GetString(1), r.GetDecimal(2), r.IsDBNull(3) ? null : r.GetString(3), new()));

        foreach (var q in questions)
        {
            using var opts = new SqlCommand(@"
                SELECT OptionId, OptionLabel, OptionText 
                FROM dbo.QuestionOptions 
                WHERE QuestionId = @id 
                ORDER BY OptionLabel", con);

            opts.Parameters.AddWithValue("@id", q.QuestionId); 
            using var r = opts.ExecuteReader();
            while (r.Read()) 
                q.Options.Add(new AnswerOption(r.GetInt32(0), r.GetString(1), r.GetString(2)));
        }

        return new QuizForTaking(quizId, title, description, duration, questions);
    }

    public QuizResult SubmitQuiz(int userId, int attemptId, QuizForTaking quiz, Dictionary<int, int> selected)
    {
        using var con = OpenConnection(); 
        con.Open(); 
        using var tx = con.BeginTransaction();
        
        try
        {
            using var verify = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM dbo.UserQuizAttempts 
                WHERE AttemptId = @attempt AND UserId = @user AND FinishedAt IS NULL", con, tx);

            verify.Parameters.AddWithValue("@attempt", attemptId); 
            verify.Parameters.AddWithValue("@user", userId);
            
            if ((int)verify.ExecuteScalar()! != 1) 
                throw new InvalidOperationException("Attempt tidak valid atau sudah selesai.");

            decimal earned = 0, max = 0;
            foreach (var q in quiz.Questions)
            {
                max += q.Weight; decimal points = 0;
                if (selected.TryGetValue(q.QuestionId, out int optionId))
                {
                    using var check = new SqlCommand(@"
                        SELECT COUNT(*) 
                        FROM dbo.QuestionOptions o 
                        JOIN dbo.Questions q ON q.QuestionId = o.QuestionId 
                        WHERE o.OptionId = @option AND q.QuestionId = @question AND q.QuizId = @quiz", con, tx);

                    check.Parameters.AddWithValue("@option", optionId); 
                    check.Parameters.AddWithValue("@question", q.QuestionId); 
                    check.Parameters.AddWithValue("@quiz", quiz.QuizId);
                    
                    if ((int)check.ExecuteScalar()! == 1)
                    {
                        using var correct = new SqlCommand(@"
                            SELECT IsCorrect 
                            FROM dbo.QuestionOptions 
                            WHERE OptionId = @id", con, tx);

                        correct.Parameters.AddWithValue("@id", optionId);
                        
                        if (Convert.ToBoolean(correct.ExecuteScalar())) 
                            points = q.Weight;

                        using var save = new SqlCommand(@"
                            INSERT INTO dbo.UserAnswers(AttemptId, QuestionId, SelectedOptionId, EarnedWeight) 
                            VALUES(@a, @q, @o, @w)", con, tx);

                        save.Parameters.AddWithValue("@a", attemptId); 
                        save.Parameters.AddWithValue("@q", q.QuestionId); 
                        save.Parameters.AddWithValue("@o", optionId); 
                        save.Parameters.AddWithValue("@w", points); 
                        save.ExecuteNonQuery();
                    }
                }
                earned += points;
            }

            decimal pct = max == 0 ? 0 : Math.Round(earned / max * 100, 2);

            using var finish = new SqlCommand(@"
                UPDATE dbo.UserQuizAttempts 
                SET FinishedAt = SYSDATETIME(), TotalEarnedWeight = @earned, FinalPercentage = @pct 
                WHERE AttemptId = @id AND UserId = @user", con, tx);

            finish.Parameters.AddWithValue("@earned", earned); 
            finish.Parameters.AddWithValue("@pct", pct); 
            finish.Parameters.AddWithValue("@id", attemptId); 
            finish.Parameters.AddWithValue("@user", userId); 
            finish.ExecuteNonQuery();

            tx.Commit(); 
            return new QuizResult(attemptId, earned, max, pct);
        }
        catch 
        { 
            tx.Rollback(); 
            throw; 
        }
    }

    public int GetLatestOpenAttemptId(int userId, int quizId)
    {
        using var con = OpenConnection(); 
        con.Open();

        using var cmd = new SqlCommand(@"
            SELECT TOP (1) AttemptId 
            FROM dbo.UserQuizAttempts 
            WHERE UserId = @user AND QuizId = @quiz AND FinishedAt IS NULL 
            ORDER BY StartedAt DESC", con);

        cmd.Parameters.AddWithValue("@user", userId); 
        cmd.Parameters.AddWithValue("@quiz", quizId);
        
        var result = cmd.ExecuteScalar();
        if (result is null) 
            throw new InvalidOperationException("Attempt kuis tidak ditemukan.");

        return Convert.ToInt32(result);
    }

}
