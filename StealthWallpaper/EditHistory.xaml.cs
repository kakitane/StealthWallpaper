using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;

namespace StealthWallpaper
{
    /// <summary>
    /// EditHistory.xaml の相互作用ロジック
    /// </summary>
    public partial class EditHistory : Window
    {
        List<string> history = new();
        public EditHistory()
        {
            InitializeComponent();
            this.history = MainWindow.GetWallpaperHistory();
            HistoryListBox.ItemsSource = history;
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryListBox.SelectedItem is null) { return; }
            string value = HistoryListBox.SelectedItem.ToString()!;
            int index = HistoryListBox.SelectedIndex;
            if (index == 0) { return; }
            history.Remove(value);
            history.Insert(index - 1, value);
            HistoryListBox.Items.Refresh();
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryListBox.SelectedItem is null) { return; }
            string value = HistoryListBox.SelectedItem.ToString()!;
            int index = HistoryListBox.SelectedIndex;
            if (index == history.Count - 1) { return; }
            history.Remove(value);
            history.Insert(index + 1, value);
            HistoryListBox.Items.Refresh();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (history.Count >= 5)
            {
                MessageBox.Show("You cannot add 5+ history.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            OpenFileDialog dialog = new()
            {
                Filter = "Image File|*.png;*.jpg"
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                history.Add(dialog.FileName);
                HistoryListBox.Items.Refresh();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryListBox.SelectedItem is null) { return; };
            history.Remove(HistoryListBox.SelectedItem.ToString()!);
            HistoryListBox.Items.Refresh();
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            history.Clear();
            HistoryListBox.Items.Refresh();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = HistoryListBox.SelectedItem.ToString(), UseShellExecute = true });
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (history.Count < 5)
            {
                for (int i = 0; i < 5 - history.Count; i++)
                {
                    history.Add(string.Empty);
                }
            }
            MainWindow.SetWallpaperHistory(history);
            this.Close();
        }
    }
}
