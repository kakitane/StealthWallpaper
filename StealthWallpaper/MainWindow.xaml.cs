using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace StealthWallpaper
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;
        public enum WallpaperStyle
        {
                Fill = 10,
                Fit = 6,
                Strech = 2,
                Tile = 0,
                Center= 0,
                Span = 22
        }
        private string wallpaperPath = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetWallpaper_Click(object sender, RoutedEventArgs e)
        {
            wallpaperPath = FilePathTextBox.Text;
            if (string.IsNullOrEmpty(wallpaperPath))
            {
                MessageBox.Show("Image is not selected. Please select it first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!File.Exists(wallpaperPath))
            {
                MessageBox.Show("Cannot access the image, please check the path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<string> history = GetWallpaperHistory();
            SetWallpaper(wallpaperPath);
            WallpaperStyle style = StyleComboBox.SelectedIndex switch
            {
                0 => WallpaperStyle.Fill,
                1 => WallpaperStyle.Fit,
                2 => WallpaperStyle.Strech,
                3 => WallpaperStyle.Tile,
                4 => WallpaperStyle.Center,
                5 => WallpaperStyle.Span,
                _ => throw new IndexOutOfRangeException("Selected item out of range.")
            };
            SetWallpaperStyle(style);

            if (StealthCheckBox.IsChecked! == true)
            {
                SetWallpaperHistory(history);
            }

            MessageBox.Show("Wallpaper set successfully!\r\nNote: If the style is not applied, right-click the desktop and select 'Refresh'.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static void SetWallpaper(string path)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        public static List<string> GetWallpaperHistory()
        {
            List<string> history = new();
            using RegistryKey? registry = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Wallpapers");
            if (registry == null) throw new Exception("Cannot access wallpaper registry");

            for (int i = 0; i < 5; i++)
            {
                object? value = registry.GetValue($"BackgroundHistoryPath{i}");
                history.Add(value?.ToString() ?? string.Empty);
            }
            return history;
        }

        public static void SetWallpaperHistory(List<string> wallpaperPaths)
        {
            using RegistryKey? registry = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Wallpapers", writable: true);
            if (registry == null) throw new Exception("Cannot access wallpaper registry");

            for (int i = 0; i < wallpaperPaths.Count; i++)
            {
                if (string.IsNullOrEmpty(wallpaperPaths[i]))
                {
                    if (registry.GetValue($"BackgroundHistoryPath{i}") != null)
                    {
                        registry.DeleteValue($"BackgroundHistoryPath{i}");
                    }
                }
                else
                {
                    registry.SetValue($"BackgroundHistoryPath{i}", wallpaperPaths[i]);
                }
            }
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e) 
        {
            OpenFileDialog dialog = new() { Filter = "Wallpaper Image|*.png;*.jpg;*.jpeg" };
            if (dialog.ShowDialog() == true)
            {
                wallpaperPath = dialog.FileName;
                FilePathTextBox.Text = wallpaperPath;
            }
        }

        public void SetWallpaperStyle(WallpaperStyle style)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true) ?? throw new Exception("Cannot access registry.");
            if (style == WallpaperStyle.Tile)
            {
                key.SetValue("TileWallpaper", 1);
            }
            else
            {

                key.SetValue("TileWallpaper", 0);
            }
            key.SetValue("WallpaperStyle", style);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            new EditHistory().ShowDialog();
        }
    }
}
