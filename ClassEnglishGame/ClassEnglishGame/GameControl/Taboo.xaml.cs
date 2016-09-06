using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ClassEnglishGame.Properties;

namespace ClassEnglishGame.GameControl
{
    /// <summary>
    /// Interaction logic for Taboo.xaml
    /// </summary>
    public partial class Taboo : INotifyPropertyChanged
    {
        private int _currentIndex;
        private List<GameItem> _gameItems;
        private int _progressBarValue;
        private GameItem _selectedGameItem;
        private Visibility _tabooHomeVisibility;
        private DispatcherTimer _timer;

        public Taboo()
        {
            InitializeComponent();
            DataContext = this;
            LoadData();
            TabooHomeVisibility = Visibility.Visible;
            CreateTimer();
            ProgressBarValue = 0;
            TimingProgress.Maximum = Settings.Default.TabooTiming*60;
            _timer.IsEnabled = false;
        }

        public Visibility TabooHomeVisibility
        {
            get { return _tabooHomeVisibility; }
            set
            {
                _tabooHomeVisibility = value;
                OnPropertyChanged("TabooHomeVisibility");
            }
        }

        public int ProgressBarValue
        {
            get { return _progressBarValue; }
            set
            {
                _progressBarValue = value;
                OnPropertyChanged("ProgressBarValue");
            }
        }

        public GameItem SelectedGameItem
        {
            get { return _selectedGameItem; }
            set
            {
                _selectedGameItem = value;
                GetIgnoreText(value);
                CurrentSelect.Text = string.Format("{0}/{1}", _currentIndex + 1, _gameItems.Count);
                OnPropertyChanged("SelectedGameItem");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void LoadData()
        {
            _gameItems =
                EnglishGameHelper.RandomOrder(
                    FileManagement.GetListGameData().Where(x => x.GameName == Constant.GameConstant.Taboo).ToList());
            if (_gameItems != null && _gameItems.Count > 0)
            {
                SelectedGameItem = _gameItems[0];
                _currentIndex = 0;
            }
        }

        private void GetIgnoreText(GameItem value)
        {
            string[] ignoreText = value.IgnoreWord.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            IgnoreWordList.Children.Clear();
            foreach (string text in ignoreText)
            {
                IgnoreWordList.Children.Add(new TextBlock
                                                {
                                                    Text = text.Trim(),
                                                    Margin = new Thickness(20, 5, 0, 5),
                                                    FontSize = 40,
                                                    Foreground = new SolidColorBrush(Colors.DarkRed),
                                                });
            }
        }

        private void StartGameClick(object sender, RoutedEventArgs e)
        {
            if (_gameItems == null || _gameItems.Count < 1)
            {
                MessageBox.Show("Cannot find data of Taboo game.", "Taboo:", MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                TabooHomeVisibility = Visibility.Visible;
            }
            else
            {
                TabooHomeVisibility = Visibility.Collapsed;
                _timer.IsEnabled = true;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PreviousClick(object sender, RoutedEventArgs e)
        {
            _timer.IsEnabled = false;
            if (_currentIndex > 0)
                _currentIndex--;
            else
                _currentIndex = _gameItems.Count - 1;

            SelectedGameItem = _gameItems[_currentIndex];
            ProgressBarValue = 0;
            _timer.IsEnabled = true;
        }

        private void NextClick(object sender, RoutedEventArgs e)
        {
            _timer.IsEnabled = false;
            if (_currentIndex < _gameItems.Count - 1)
                _currentIndex++;
            else
                _currentIndex = 0;

            SelectedGameItem = _gameItems[_currentIndex];
            ProgressBarValue = 0;
            _timer.IsEnabled = true;
        }

        private void CreateTimer()
        {
            if (_timer != null) return;
            _timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 1, 0)}; //1s
            _timer.Tick += (o, s) =>
                               {
                                   EnglishGameHelper.SpeakNumberBetween(
                                       (int) (TimingProgress.Maximum - ProgressBarValue));
                                   if (TimingProgress.Maximum > ProgressBarValue)
                                   {
                                       ProgressBarValue++;
                                   }
                                   else
                                   {
                                       _timer.IsEnabled = false;
                                       //Play sound
                                       //new SoundPlayer(Constant.FileAudioName).Play();
                                       new SoundPlayer(Properties.Resources.beep).Play();
                                   }
                               };
        }

        private void CopyAllWordClick(object sender, RoutedEventArgs e)
        {
            string wordList = string.Join(";", _gameItems.Select(x => x.Title).ToArray());
            Clipboard.SetText(wordList);
        }
    }
}