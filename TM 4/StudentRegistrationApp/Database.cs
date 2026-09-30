using Microsoft.Data.Sqlite;

namespace StudentRegistrationApp
{
    public static class Database
    {
        private const string ConnectionString =
            "Data Source=mahasiswa.db";

        public static void Initialize()
        {
            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS mahasiswa (
                    nrp TEXT PRIMARY KEY,
                    nama TEXT NOT NULL,
                    jurusan TEXT NOT NULL,
                    angkatan INTEGER NOT NULL
                );
            ";

            command.ExecuteNonQuery();
        }

        public static SqliteConnection GetConnection()
        {
            var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            return connection;
        }
    }
}