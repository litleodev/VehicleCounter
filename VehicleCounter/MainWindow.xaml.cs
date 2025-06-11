using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace VehicleCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isLoadedVideo = false;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PlayButton.IsEnabled = false;
            PauseButton.Visibility = Visibility.Collapsed;
            RewindButton.IsEnabled = false;
            FastForwardButton.IsEnabled = false;
            VideoSlider.Value = 0;
            VideoSlider.IsEnabled = false;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (SpeedComboBox.SelectedItem != null)
            {
                string selectedSpeed = (SpeedComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                double speed = double.Parse(selectedSpeed.Replace("x", ""));
                VideoPlayer.SpeedRatio = speed;
            }
            VideoPlayer.Play();
            timer.Start();
            PlayButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Visible;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.SpeedRatio = 1;
            VideoPlayer.Pause();
            timer.Stop();
            PauseButton.Visibility = Visibility.Collapsed;
            PlayButton.Visibility = Visibility.Visible;
        }

        private void RewindButton_Click(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan newPosition = VideoPlayer.Position.Subtract(TimeSpan.FromSeconds(10));
                if (newPosition.TotalSeconds < 0)
                {
                    newPosition = TimeSpan.Zero;
                }
                VideoPlayer.Position = newPosition;
            }
        }

        private void FastForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan newPosition = VideoPlayer.Position.Add(TimeSpan.FromSeconds(10));
                if (newPosition > VideoPlayer.NaturalDuration.TimeSpan)
                {
                    newPosition = VideoPlayer.NaturalDuration.TimeSpan;
                }
                VideoPlayer.Position = newPosition;
            }
        }

        private async void VideoSlider_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                VideoPlayer.Position = TimeSpan.FromSeconds(VideoSlider.Value - 0.001);
                VideoPlayer.Play();
                await Task.Delay(1);
                VideoPlayer.Pause();
                CurrentTimeTextBlock.Text = VideoPlayer.Position.ToString(@"hh\:mm\:ss");
            }
        }

        private async void SelectVideoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoadedVideo)
            {
                VideoPlayer.Stop();
                VideoPlayer.Source = null;
                _isLoadedVideo = false;
                PlayButton.IsEnabled = false;
                VideoSlider.IsEnabled = false;
                VideoSlider.Value = 0;
                CurrentTimeTextBlock.Text = "00:00:00";
                TotalTimeTextBlock.Text = "00:00:00";
            }

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
                VideoPlayer.MediaOpened += VideoPlayer_MediaOpened;
                VideoPlayer.Position = TimeSpan.FromSeconds(0);
                VideoPlayer.Play();
                await Task.Delay(100);
                VideoPlayer.Pause();

                if (!_isLoadedVideo)
                {
                    PlayButton.IsEnabled = true;
                    VideoSlider.Value = 0;
                    VideoSlider.IsEnabled = true;
                    RewindButton.IsEnabled = true;
                    FastForwardButton.IsEnabled = true;
                    timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(100)
                    };
                    timer.Tick += Timer_Tick;
                    timer.Start();
                }
                PlayButton.Visibility = Visibility.Visible;

                _isLoadedVideo = true;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                VideoSlider.Value = VideoPlayer.Position.TotalSeconds;
                CurrentTimeTextBlock.Text = VideoPlayer.Position.ToString(@"hh\:mm\:ss");
            }
            else
            {
                timer.Stop();
            }
        }

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                VideoSlider.Maximum = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                TotalTimeTextBlock.Text = VideoPlayer.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");
            }
        }

        private void SpeedComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoadedVideo && SpeedComboBox.SelectedItem != null)
            {
                string selectedSpeed = (SpeedComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                double speed = double.Parse(selectedSpeed.Replace("x", ""));
                VideoPlayer.SpeedRatio = speed;
            }
        }
    }
}
