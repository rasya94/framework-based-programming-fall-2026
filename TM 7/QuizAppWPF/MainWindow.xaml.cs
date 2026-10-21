using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using QuizAppWPF.Data;
using QuizAppWPF.Data.Attempts;
using QuizAppWPF.Data.Quizzes;
using QuizAppWPF.Data.Teachers;
using QuizAppWPF.Data.Users;
using QuizAppWPF.Models;

namespace QuizAppWPF;

public partial class MainWindow : Window
{
    private readonly QuizRepository _quizRepository;
    private readonly UserRepository _userRepository;
    private readonly AttemptRepository _attemptRepository;
    private readonly TeacherRepository _teacherRepository;
    private UserAccount? _user;
    private QuizForTaking? _currentQuiz;
    private int _currentAttemptId;
    private readonly Dictionary<int, RadioButton> _selectedOptions = new();

    public MainWindow()
    {
        InitializeComponent();

        var settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        using var json = JsonDocument.Parse(File.ReadAllText(settingsPath));

        var connectionString = json.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("DefaultConnection")
            .GetString()!;

        _quizRepository = new QuizRepository(connectionString);
        _userRepository = new UserRepository(connectionString);
        _attemptRepository = new AttemptRepository(connectionString);
        _teacherRepository = new TeacherRepository(connectionString);
    }

    private void HandleLogin(object sender, RoutedEventArgs e)
    {
        try
        {
            _user = _userRepository.Login(UsernameBox.Text, PasswordBox.Password);

            if (_user is null)
            {
                LoginMessage.Text = "Username atau password salah.";
                return;
            }

            UsernameText.Text = $"@{_user.Username}";
            CatalogTab.Content = _user.IsTeacher ? "Semua kuis" : "Katalog kuis";
            HistoryTab.Visibility = _user.IsTeacher ? Visibility.Collapsed : Visibility.Visible;
            LoginPanel.Visibility = Visibility.Collapsed;
            MainPanel.Visibility = Visibility.Visible;

            if (_user.IsTeacher) ShowTeacherQuizzes();
            else ShowCatalog();
        }
        catch (Exception ex)
        {
            LoginMessage.Text = "Tidak dapat terhubung ke database.\n" + ex.Message;
        }
    }

    private void HandleLogout(object sender, RoutedEventArgs e)
    {
        _user = null;
        _currentQuiz = null;
        _selectedOptions.Clear();
        CatalogTab.Content = "Katalog kuis";
        HistoryTab.Visibility = Visibility.Visible;

        MainPanel.Visibility = Visibility.Collapsed;
        LoginPanel.Visibility = Visibility.Visible;

        PasswordBox.Clear();
        LoginMessage.Text = "";
    }

    private void HandleCatalog(object sender, RoutedEventArgs e) => ShowCatalog();

    private void HandleHistory(object sender, RoutedEventArgs e) => ShowHistory();

    private void ShowCatalog()
    {
        if (_user?.IsTeacher == true) { ShowTeacherQuizzes(); return; }
        TopHeader.Visibility = Visibility.Visible;
        SetActiveTab(catalog: true);

        TransitionContent(() =>
        {
            ContentPanel.Children.Clear();

            UiHelpers.AddHeading(ContentPanel, "Katalog kuis", "Pilih kuis aktif untuk mulai mengerjakan.");

            try
            {
                var quizzes = _quizRepository.GetActiveQuizzes();
                var completedQuizIds = _quizRepository.GetCompletedQuizIds(_user!.UserId);

                if (quizzes.Count == 0)
                {
                    UiHelpers.AddInfo(ContentPanel, "Belum ada kuis aktif.");
                    return;
                }

                foreach (var quiz in quizzes)
                {
                    var completed = completedQuizIds.Contains(quiz.QuizId);
                    var card = UiHelpers.NewCard();
                    var stack = new StackPanel();

                    stack.Children.Add(new TextBlock
                    {
                        Text = quiz.Title,
                        FontSize = 20,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = UiHelpers.Brush("#172033")
                    });

                    stack.Children.Add(new TextBlock
                    {
                        Text = string.IsNullOrWhiteSpace(quiz.Description) ? "Tidak ada deskripsi." : quiz.Description,
                        Margin = new Thickness(0, 8, 0, 12),
                        Foreground = UiHelpers.Brush("#667085"),
                        TextWrapping = TextWrapping.Wrap
                    });

                    stack.Children.Add(new TextBlock
                    {
                        Text = $"{quiz.QuestionCount} soal  •  {quiz.DurationMinutes} menit  •  Bobot maksimum {quiz.MaxWeight:0.##}",
                        Foreground = UiHelpers.Brush("#475467")
                    });

                    var start = new Button
                    {
                        Content = completed ? "Sudah dikerjakan" : "Mulai kuis",
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0, 16, 0, 0),
                        Tag = quiz.QuizId,
                        IsEnabled = !completed,
                        Background = completed ? UiHelpers.Brush("#98A2B3") : UiHelpers.Brush("#6669E3")
                    };

                    start.Click += TryStartQuiz;

                    stack.Children.Add(start);
                    card.Child = stack;
                    ContentPanel.Children.Add(card);
                }
            }
            catch (Exception ex)
            {
                UiHelpers.AddInfo(ContentPanel, "Gagal memuat katalog: " + ex.Message);
            }
        });
    }

    private void TryStartQuiz(object sender, RoutedEventArgs e)
    {
        if (_user is null || _user.IsTeacher || sender is not Button { Tag: int quizId })
            return;

        try
        {
            _currentQuiz = _quizRepository.StartQuiz(_user.UserId, quizId);

            if (_currentQuiz is null)
            {
                MessageBox.Show("Kuis tidak ditemukan atau tidak aktif.");
                ShowCatalog();
                return;
            }

            _currentAttemptId = _quizRepository.GetLatestOpenAttemptId(_user.UserId, quizId);
            _selectedOptions.Clear();
            TopHeader.Visibility = Visibility.Collapsed;

            ShowQuiz();
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message, "Kuis tidak dapat dimulai", MessageBoxButton.OK, MessageBoxImage.Information);
            ShowCatalog();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Kuis gagal dibuka: " + ex.Message);
        }
    }

    private void ShowQuiz()
    {
        if (_currentQuiz is null)
            return;

        TransitionContent(() =>
        {
            ContentPanel.Children.Clear();

            UiHelpers.AddHeading(ContentPanel, _currentQuiz.Title, $"Durasi: {_currentQuiz.DurationMinutes} menit. Pilih satu jawaban untuk setiap soal.");

            if (_currentQuiz.Questions.Count == 0)
            {
                UiHelpers.AddInfo(ContentPanel, "Kuis ini belum memiliki soal.");
                return;
            }

            for (var index = 0; index < _currentQuiz.Questions.Count; index++)
            {
                var q = _currentQuiz.Questions[index];
                var card = UiHelpers.NewCard();
                var stack = new StackPanel();

                stack.Children.Add(new TextBlock
                {
                    Text = $"Soal {index + 1}  •  Bobot {q.Weight:0.##}",
                    Foreground = UiHelpers.Brush("#6669E3"),
                    FontWeight = FontWeights.SemiBold,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = q.QuestionText,
                    FontSize = 17,
                    FontWeight = FontWeights.SemiBold,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = UiHelpers.Brush("#172033"),
                    Margin = new Thickness(0, 0, 0, 14)
                });

                AddQuestionImage(stack, q.ImagePath);

                foreach (var option in q.Options)
                {
                    var radio = new RadioButton
                    {
                        Content = $"{option.OptionLabel}. {option.OptionText}",
                        GroupName = "question_" + q.QuestionId,
                        Tag = option.OptionId,
                        FontSize = 14,
                        Margin = new Thickness(4, 8, 4, 8),
                        Padding = new Thickness(4),
                        Foreground = UiHelpers.Brush("#344054")
                    };

                    stack.Children.Add(radio);

                    radio.Checked += (_, _) => _selectedOptions[q.QuestionId] = radio;
                }

                card.Child = stack;
                ContentPanel.Children.Add(card);
            }

            var submit = new Button
            {
                Content = "Kumpulkan jawaban",
                HorizontalAlignment = HorizontalAlignment.Left,
                Padding = new Thickness(22, 12, 22, 12),
                Margin = new Thickness(0, 8, 0, 28)
            };

            submit.Click += TrySubmitQuiz;
            ContentPanel.Children.Add(submit);
        });
    }

    private void TrySubmitQuiz(object sender, RoutedEventArgs e)
    {
        if (_user is null || _currentQuiz is null)
            return;

        if (MessageBox.Show("Kumpulkan jawaban sekarang?", "Konfirmasi", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        var answers = _selectedOptions
            .Where(kv => kv.Value.IsChecked == true && kv.Value.Tag is int)
            .ToDictionary(kv => kv.Key, kv => (int)kv.Value.Tag);

        try
        {
            var result = _quizRepository.SubmitQuiz(_user.UserId, _currentAttemptId, _currentQuiz, answers);

            _currentQuiz = null;
            _selectedOptions.Clear();
            TopHeader.Visibility = Visibility.Visible;

            TransitionContent(() =>
            {
                ContentPanel.Children.Clear();

                UiHelpers.AddHeading(ContentPanel, "Kuis selesai", "Jawaban berhasil dikumpulkan.");

                var card = UiHelpers.NewCard();
                var stack = new StackPanel();

                stack.Children.Add(new TextBlock
                {
                    Text = $"{result.Percentage:0.##}%",
                    FontSize = 44,
                    FontWeight = FontWeights.Bold,
                    Foreground = UiHelpers.Brush("#6669E3")
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"Skor: {result.EarnedWeight:0.##} dari {result.MaxWeight:0.##}",
                    FontSize = 18,
                    Foreground = UiHelpers.Brush("#475467"),
                    Margin = new Thickness(0, 8, 0, 0)
                });

                card.Child = stack;
                ContentPanel.Children.Add(card);

                var review = new Button
                {
                    Content = "Tinjau jawaban",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 0, 8, 0),
                    Tag = result.AttemptId
                };

                review.Click += (_, _) => ShowReview(result.AttemptId, returnsToHistory: false);

                var catalog = new Button
                {
                    Content = "Kembali ke katalog"
                };

                catalog.Click += (_, _) => ShowCatalog();

                var actions = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                actions.Children.Add(review);
                actions.Children.Add(catalog);
                ContentPanel.Children.Add(actions);
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal mengumpulkan kuis: " + ex.Message);
        }
    }

    private void ShowHistory()
    {
        if (_user?.IsTeacher == true) return;
        TopHeader.Visibility = Visibility.Visible;
        SetActiveTab(catalog: false);

        TransitionContent(() =>
        {
            ContentPanel.Children.Clear();

            UiHelpers.AddHeading(ContentPanel, "Riwayat pengerjaan", "Nilai dari kuis yang pernah dikerjakan.");

            try
            {
                var history = _attemptRepository.GetHistory(_user!.UserId);

                if (history.Count == 0)
                {
                    UiHelpers.AddInfo(ContentPanel, "Belum ada riwayat kuis.");
                    return;
                }

                foreach (var item in history)
                {
                    var card = UiHelpers.NewCard();
                    var stack = new StackPanel();

                    stack.Children.Add(new TextBlock
                    {
                        Text = item.Title,
                        FontSize = 18,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = UiHelpers.Brush("#172033")
                    });

                    stack.Children.Add(new TextBlock
                    {
                        Text = item.FinishedAt.HasValue ? item.FinishedAt.Value.ToString("dd MMM yyyy, HH:mm") : "Belum selesai",
                        Foreground = UiHelpers.Brush("#667085"),
                        Margin = new Thickness(0, 8, 0, 10)
                    });

                    stack.Children.Add(new TextBlock
                    {
                        Text = $"{item.Percentage:0.##}%  ·  Skor {item.EarnedWeight:0.##}/{item.MaxWeight:0.##}",
                        Foreground = UiHelpers.Brush("#6669E3"),
                        FontWeight = FontWeights.SemiBold
                    });

                    var review = new Button
                    {
                        Content = "Tinjau jawaban",
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0, 14, 0, 0),
                        Tag = item.AttemptId,
                        IsEnabled = item.FinishedAt.HasValue
                    };

                    review.Click += ReviewAttempt_Click;

                    stack.Children.Add(review);
                    card.Child = stack;
                    ContentPanel.Children.Add(card);
                }
            }
            catch (Exception ex)
            {
                UiHelpers.AddInfo(ContentPanel, "Gagal memuat riwayat: " + ex.Message);
            }
        });
    }

    private void ShowTeacherQuizzes()
    {
        if (_user?.IsTeacher != true) return;
        TopHeader.Visibility = Visibility.Visible;
        SetActiveTab(catalog: true);
        TransitionContent(() =>
        {
            ContentPanel.Children.Clear();
            UiHelpers.AddHeading(ContentPanel, "Semua kuis", "Pilih kuis untuk melihat jawaban dan hasil setiap peserta.");
            try
            {
                var quizzes = _teacherRepository.GetAllQuizzes();
                if (quizzes.Count == 0) { UiHelpers.AddInfo(ContentPanel, "Belum ada kuis."); return; }
                foreach (var quiz in quizzes)
                {
                    var card = UiHelpers.NewCard();
                    var stack = new StackPanel();
                    stack.Children.Add(new TextBlock { Text = quiz.Title, FontSize = 20, FontWeight = FontWeights.SemiBold, Foreground = UiHelpers.Brush("#172033") });
                    stack.Children.Add(new TextBlock
                    {
                        Text = $"{(quiz.IsActive ? "Aktif" : "Tidak aktif")}  ·  {quiz.QuestionCount} soal  ·  {quiz.DurationMinutes} menit  ·  {quiz.AttemptCount} pengerjaan",
                        Foreground = UiHelpers.Brush("#667085"), Margin = new Thickness(0, 8, 0, 12)
                    });
                    if (!string.IsNullOrWhiteSpace(quiz.Description))
                        stack.Children.Add(new TextBlock { Text = quiz.Description, Foreground = UiHelpers.Brush("#475467"), TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 12) });
                    var answers = new Button { Content = "Lihat jawaban", HorizontalAlignment = HorizontalAlignment.Left, Tag = quiz };
                    answers.Click += TryViewQuizTeacher;
                    stack.Children.Add(answers); card.Child = stack; ContentPanel.Children.Add(card);
                }
            }
            catch (Exception ex) { UiHelpers.AddInfo(ContentPanel, "Gagal memuat kuis: " + ex.Message); }
        });
    }

    private void TryViewQuizTeacher(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: TeacherQuizSummary quiz }) ShowTeacherAnswers(quiz);
    }

    private void ShowTeacherAnswers(TeacherQuizSummary quiz)
    {
        if (_user?.IsTeacher != true) return;
        TopHeader.Visibility = Visibility.Visible;
        TransitionContent(() =>
        {
            ContentPanel.Children.Clear();
            var back = new Button { Content = "← Kembali ke semua kuis", HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0, 0, 0, 18), Background = UiHelpers.Brush("#F3F4F6"), Foreground = UiHelpers.Brush("#374151") };
            back.Click += (_, _) => ShowTeacherQuizzes(); ContentPanel.Children.Add(back);
            try
            {
                var rows = _teacherRepository.GetQuizAnswers(quiz.QuizId);
                UiHelpers.AddHeading(ContentPanel, $"Jawaban: {quiz.Title}", $"{rows.Select(row => row.AttemptId).Distinct().Count()} pengerjaan · jawaban benar dan salah, termasuk soal yang belum dijawab.");
                if (rows.Count == 0) { UiHelpers.AddInfo(ContentPanel, "Belum ada peserta yang mengerjakan kuis ini."); return; }
                foreach (var attempt in rows.GroupBy(row => row.AttemptId))
                {
                    var first = attempt.First();
                    var card = UiHelpers.NewCard(); var stack = new StackPanel();
                    stack.Children.Add(new TextBlock { Text = $"{first.FullName} (@{first.Username})", FontSize = 18, FontWeight = FontWeights.SemiBold, Foreground = UiHelpers.Brush("#172033") });
                    stack.Children.Add(new TextBlock
                    {
                        Text = first.FinishedAt.HasValue
                            ? $"Skor akhir: {first.AttemptEarnedWeight:0.##}/{first.AttemptMaxWeight:0.##} ({first.AttemptPercentage:0.##}%)  ·  {first.FinishedAt.Value:dd MMM yyyy, HH:mm}"
                            : $"Belum dikumpulkan  ·  mulai {first.StartedAt:dd MMM yyyy, HH:mm}",
                        Foreground = first.FinishedAt.HasValue ? UiHelpers.Brush("#6669E3") : UiHelpers.Brush("#667085"),
                        FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 6, 0, 14)
                    });
                    var questionNumber = 0;
                    foreach (var answer in attempt)
                    {
                        questionNumber++;
                        var state = answer.IsCorrect switch { true => "Benar", false => "Salah", null => "Belum dijawab" };
                        var stateColor = answer.IsCorrect switch { true => UiHelpers.Brush("#15803D"), false => UiHelpers.Brush("#B42318"), null => UiHelpers.Brush("#667085") };
                        var question = new StackPanel { Margin = new Thickness(0, 0, 0, 12) };
                        question.Children.Add(new TextBlock { Text = $"{questionNumber}. {answer.QuestionText}", FontWeight = FontWeights.SemiBold, TextWrapping = TextWrapping.Wrap, Foreground = UiHelpers.Brush("#344054") });
                        question.Children.Add(new TextBlock { Text = $"Jawaban: {answer.SelectedOptionText ?? "(tidak dijawab)"} · {state}", TextWrapping = TextWrapping.Wrap, Foreground = stateColor, Margin = new Thickness(0, 4, 0, 0) });
                        if (answer.IsCorrect == false && answer.CorrectOptionText is not null)
                            question.Children.Add(new TextBlock { Text = $"Jawaban benar: {answer.CorrectOptionText}", TextWrapping = TextWrapping.Wrap, Foreground = UiHelpers.Brush("#475467"), Margin = new Thickness(0, 3, 0, 0) });
                        stack.Children.Add(question);
                    }
                    card.Child = stack; ContentPanel.Children.Add(card);
                }
            }
            catch (Exception ex) { UiHelpers.AddInfo(ContentPanel, "Gagal memuat jawaban: " + ex.Message); }
        });
    }

    private void ReviewAttempt_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: int attemptId })
            ShowReview(attemptId, returnsToHistory: true);
    }

    private void ShowReview(int attemptId, bool returnsToHistory)
    {
        TopHeader.Visibility = Visibility.Visible;

        TransitionContent(() =>
        {
            ContentPanel.Children.Clear();

            try
            {
                var review = _attemptRepository.GetAttemptReview(_user!.UserId, attemptId);

                if (review is null)
                {
                    UiHelpers.AddInfo(ContentPanel, "Percobaan selesai tidak ditemukan.");
                    return;
                }

                UiHelpers.AddHeading(
                    ContentPanel,
                    $"{(returnsToHistory ? "Tinjauan jawaban" : "Kuis selesai")}: {review.Title}",
                    $"{review.Percentage:0.##}%  ·  Skor {review.EarnedWeight:0.##}/{review.MaxWeight:0.##}. Mode baca saja; jawaban tidak dapat diubah."
                );

                foreach (var (q, index) in review.Questions.Select((question, index) => (question, index)))
                {
                    var correct = q.EarnedWeight > 0;
                    var card = UiHelpers.NewCard();
                    var stack = new StackPanel();

                    stack.Children.Add(new TextBlock
                    {
                        Text = $"Soal {index + 1}  ·  " + (correct ? "Benar" : "Salah / belum dijawab"),
                        Foreground = correct ? UiHelpers.Brush("#15803D") : UiHelpers.Brush("#B42318"),
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 0, 0, 10)
                    });

                    stack.Children.Add(new TextBlock
                    {
                        Text = q.QuestionText,
                        FontSize = 17,
                        FontWeight = FontWeights.SemiBold,
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = UiHelpers.Brush("#172033"),
                        Margin = new Thickness(0, 0, 0, 14)
                    });

                    AddQuestionImage(stack, q.ImagePath);

                    foreach (var option in q.Options)
                    {
                        var status = option.IsCorrect ? " ✓ Jawaban benar" : option.IsSelected ? "  ·  Jawaban Anda" : "";
                        var color = option.IsCorrect ? "#15803D" : option.IsSelected ? "#B42318" : "#475467";

                        stack.Children.Add(new TextBlock
                        {
                            Text = $"{option.OptionLabel}. {option.OptionText}{status}",
                            TextWrapping = TextWrapping.Wrap,
                            Foreground = UiHelpers.Brush(color),
                            Margin = new Thickness(4, 7, 4, 7),
                            FontWeight = option.IsCorrect || option.IsSelected ? FontWeights.SemiBold : FontWeights.Normal
                        });
                    }

                    if (!q.Options.Any(o => o.IsSelected))
                    {
                        stack.Children.Add(new TextBlock
                        {
                            Text = "Anda tidak memilih jawaban.",
                            Foreground = UiHelpers.Brush("#B42318"),
                            Margin = new Thickness(4, 8, 4, 0)
                        });
                    }

                    card.Child = stack;
                    ContentPanel.Children.Add(card);
                }
            }
            catch (Exception ex)
            {
                UiHelpers.AddInfo(ContentPanel, "Gagal memuat tinjauan: " + ex.Message);
            }
        });
    }

    private void AddQuestionImage(StackPanel panel, string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return;

        string path = Path.IsPathRooted(imagePath)
            ? imagePath
            : Path.Combine(AppContext.BaseDirectory, imagePath.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar));

        if (!File.Exists(path))
        {
            panel.Children.Add(new TextBlock
            {
                Text = $"Gambar tidak ditemukan: {path}",
                Foreground = Brushes.Red,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 8, 0, 12)
            });
            return;
        }

        try
        {
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri(Path.GetFullPath(path), UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();

            panel.Children.Add(new Image
            {
                Source = bitmap,
                MaxWidth = 500,
                MaxHeight = 280,
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 12)
            });
        }
        catch (Exception ex)
        {
            panel.Children.Add(new TextBlock
            {
                Text = $"Gambar gagal dimuat: {ex.Message}",
                Foreground = Brushes.Red,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 8, 0, 12)
            });
        }
    }

    private void TransitionContent(Action render)
    {
        var fadeOut = new DoubleAnimation(ContentPanel.Opacity, 0, TimeSpan.FromMilliseconds(90));

        fadeOut.Completed += (_, _) =>
        {
            render();

            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(180));
            ContentPanel.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        };

        ContentPanel.BeginAnimation(UIElement.OpacityProperty, fadeOut);
    }

    private void SetActiveTab(bool catalog)
    {
        CatalogTab.Background = catalog ? UiHelpers.Brush("#6669E3") : UiHelpers.Brush("#F2F4F7");
        CatalogTab.Foreground = catalog ? Brushes.White : UiHelpers.Brush("#475467");

        HistoryTab.Background = catalog ? UiHelpers.Brush("#F2F4F7") : UiHelpers.Brush("#6669E3");
        HistoryTab.Foreground = catalog ? UiHelpers.Brush("#475467") : Brushes.White;
    }
}


