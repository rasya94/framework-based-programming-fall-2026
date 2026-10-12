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

using StudentRegistrationApp.Repositories;

namespace StudentRegistrationApp;

public partial class MainWindow : Window
{
    private readonly StudentRepository _studentRepository;

    public MainWindow()
    {
        InitializeComponent();

        _studentRepository = new StudentRepository();

        LoadStudents();
    }

    private void LoadStudents()
    {
        try
        {
            var students = _studentRepository.GetAll();

            StudentDataGrid.ItemsSource = students;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Database Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void RefreshButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        LoadStudents();
    }
}