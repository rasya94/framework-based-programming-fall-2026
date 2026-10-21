using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace QuizAppWPF;

internal static class UiHelpers
{
    public static void Transition(Window current, Window next)
    {
        next.Opacity = 0;
        next.Show();
        next.Activate();

        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(160));
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
        fadeOut.Completed += (_, _) =>
        {
            current.Hide();
            current.BeginAnimation(Window.OpacityProperty, null);
            current.Opacity = 1;
        };
        current.BeginAnimation(Window.OpacityProperty, fadeOut);
        next.BeginAnimation(Window.OpacityProperty, fadeIn);
    }

    public static SolidColorBrush Brush(string color) => new((Color)ColorConverter.ConvertFromString(color));
    public static Border NewCard() => new() { Background = Brushes.White, CornerRadius = new CornerRadius(12), Padding = new Thickness(26), Margin = new Thickness(0, 0, 0, 20), BorderBrush = Brush("#E8EBF0"), BorderThickness = new Thickness(1) };
    public static void AddHeading(Panel panel, string title, string subtitle)
    {
        panel.Children.Add(new TextBlock { Text = title, FontSize = 30, FontWeight = FontWeights.Bold, Foreground = Brush("#172033"), Margin = new Thickness(0, 0, 0, 8) });
        panel.Children.Add(new TextBlock { Text = subtitle, FontSize = 14, Foreground = Brush("#667085"), Margin = new Thickness(0, 0, 0, 28), TextWrapping = TextWrapping.Wrap });
    }
    public static void AddInfo(Panel panel, string text) => panel.Children.Add(new Border { Background = Brushes.White, CornerRadius = new CornerRadius(10), Padding = new Thickness(26), Child = new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap, Foreground = Brush("#475467") } });
}
