using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace VehicleCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isLoadedVideo = false;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PlayButton.IsEnabled = false;
            PauseButton.Visibility = Visibility.Collapsed;
            VideoSlider.Value = 0;
            VideoSlider.IsEnabled = false;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Play();
            PlayButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Visible;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Pause();
            PauseButton.Visibility = Visibility.Collapsed;
            PlayButton.Visibility = Visibility.Visible;
        }

        private void VideoSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private void SelectVideoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Video Files|*.mp4;*.avi;*.mov;*.mkv|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedPath = openFileDialog.FileName;
                FilePathTextBox.Text = selectedPath;

                VideoPlayer.Source = new Uri(selectedPath, UriKind.Absolute);
                VideoPlayer.LoadedBehavior = MediaState.Manual;
                VideoPlayer.UnloadedBehavior = MediaState.Manual;

                VideoPlayer.Position = TimeSpan.FromSeconds(0);
                VideoPlayer.Pause();

                if (!_isLoadedVideo)
                {
                    PlayButton.IsEnabled = true;
                    VideoSlider.Value = 0;
                    VideoSlider.IsEnabled = true;
                }
                PlayButton.Visibility = Visibility.Visible;

                _isLoadedVideo = true;
            }
        }
    }
}
