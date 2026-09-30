using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Data.Sqlite;
using System.Data;

namespace StudentRegistrationApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TampilkanSemua();
        }

        private void TampilkanSemua()
        {
            using var connection = Database.GetConnection();

            var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT
                    nrp AS NRP,
                    nama AS Nama,
                    jurusan AS Jurusan,
                    angkatan AS Angkatan
                FROM mahasiswa
                ORDER BY nrp;
            ";

            using var reader = command.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(reader);

            dataMahasiswa.ItemsSource = table.DefaultView;
        }

        private void Tambah_Click(
            object sender,
            RoutedEventArgs e)
        {
            string nrp = txtNRP.Text.Trim();
            string nama = txtNama.Text.Trim();
            string jurusan = txtJurusan.Text.Trim();
            string angkatanText = txtAngkatan.Text.Trim();

            if (nrp.Length != 10 ||
                !nrp.All(char.IsDigit))
            {
                MessageBox.Show(
                    "NRP harus terdiri dari tepat 10 digit.");

                return;
            }

            if (string.IsNullOrWhiteSpace(nama))
            {
                MessageBox.Show("Nama harus diisi.");
                return;
            }

            if (string.IsNullOrWhiteSpace(jurusan))
            {
                MessageBox.Show("Jurusan harus diisi.");
                return;
            }

            if (!int.TryParse(
                    angkatanText,
                    out int angkatan))
            {
                MessageBox.Show(
                    "Angkatan harus berupa angka.");

                return;
            }

            try
            {
                using var connection =
                    Database.GetConnection();

                var command =
                    connection.CreateCommand();

                command.CommandText = @"
                    INSERT INTO mahasiswa
                    (nrp, nama, jurusan, angkatan)
                    VALUES
                    ($nrp, $nama, $jurusan, $angkatan);
                ";

                command.Parameters.AddWithValue(
                    "$nrp", nrp);

                command.Parameters.AddWithValue(
                    "$nama", nama);

                command.Parameters.AddWithValue(
                    "$jurusan", jurusan);

                command.Parameters.AddWithValue(
                    "$angkatan", angkatan);

                command.ExecuteNonQuery();

                MessageBox.Show(
                    "Data mahasiswa berhasil ditambahkan.");

                BersihkanForm();
                TampilkanSemua();
            }
            catch (SqliteException)
            {
                MessageBox.Show(
                    "NRP tersebut sudah terdaftar.");
            }
        }

        private void Hapus_Click(
            object sender,
            RoutedEventArgs e)
        {
            string nrp = txtNRP.Text.Trim();

            if (nrp.Length != 10 ||
                !nrp.All(char.IsDigit))
            {
                MessageBox.Show(
                    "Masukkan NRP 10 digit.");

                return;
            }

            using var connection =
                Database.GetConnection();

            var command =
                connection.CreateCommand();

            command.CommandText = @"
                DELETE FROM mahasiswa
                WHERE nrp = $nrp;
            ";

            command.Parameters.AddWithValue(
                "$nrp", nrp);

            int result = command.ExecuteNonQuery();

            if (result > 0)
            {
                MessageBox.Show(
                    "Data mahasiswa berhasil dihapus.");

                BersihkanForm();
                TampilkanSemua();
            }
            else
            {
                MessageBox.Show(
                    "Mahasiswa tidak ditemukan.");
            }
        }

        private void Cari_Click(
            object sender,
            RoutedEventArgs e)
        {
            string nrp = txtCari.Text.Trim();

            if (nrp.Length != 10 ||
                !nrp.All(char.IsDigit))
            {
                MessageBox.Show(
                    "NRP harus terdiri dari 10 digit.");

                return;
            }

            using var connection =
                Database.GetConnection();

            var command =
                connection.CreateCommand();

            command.CommandText = @"
                SELECT
                    nrp AS NRP,
                    nama AS Nama,
                    jurusan AS Jurusan,
                    angkatan AS Angkatan
                FROM mahasiswa
                WHERE nrp = $nrp;
            ";

            command.Parameters.AddWithValue(
                "$nrp", nrp);

            using var reader =
                command.ExecuteReader();

            DataTable table = new DataTable();

            table.Load(reader);

            if (table.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Mahasiswa tidak ditemukan.");
            }

            dataMahasiswa.ItemsSource =
                table.DefaultView;
        }

        private void TampilkanSemua_Click(
            object sender,
            RoutedEventArgs e)
        {
            txtCari.Clear();

            TampilkanSemua();
        }

        private void BersihkanForm()
        {
            txtNRP.Clear();
            txtNama.Clear();
            txtJurusan.Clear();
            txtAngkatan.Clear();
        }
    }
}