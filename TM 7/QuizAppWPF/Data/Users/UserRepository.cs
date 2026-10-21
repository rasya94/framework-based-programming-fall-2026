using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using QuizAppWPF.Models;

namespace QuizAppWPF.Data.Users;

public sealed class UserRepository
{
    private readonly string _connectionString;
    public UserRepository(string connectionString) => _connectionString = connectionString;

    public UserAccount? Login(string username, string password)
    {
        using var con = new SqlConnection(_connectionString);
        con.Open();
        using var cmd = new SqlCommand(@"
            SELECT UserId,Username,FullName,Role,PasswordHash
            FROM dbo.Users WHERE Username=@username", con);
        cmd.Parameters.Add("@username", System.Data.SqlDbType.NVarChar, 50).Value = username.Trim();
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(hash), Encoding.UTF8.GetBytes(r.GetString(4)))) return null;
        return new UserAccount(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3));
    }
}
