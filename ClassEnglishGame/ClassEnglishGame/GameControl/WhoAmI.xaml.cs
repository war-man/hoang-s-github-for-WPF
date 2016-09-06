using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ClassEnglishGame.Properties;

namespace ClassEnglishGame.GameControl
{
    /// <summary>
    /// Interaction logic for WhoAmI.xaml
    /// </summary>
    public partial class WhoAmI : INotifyPropertyChanged,IDisposable
    {
        private int _currentIndex;
        private List<GameItem> _gameItems;
        private int _progressBarValue;
        private GameItem _selectedGameItem;
        private DispatcherTimer _timer;
        private Visibility _whoAmIHomeVisibility;

        public WhoAmI()
        {
            InitializeComponent();
            DataContext = this;
            LoadData();
            WhoAmIHomeVisibility = Visibility.Visible;
            CreateTimer();
            ProgressBarValue = 0;
            TimingProgress.Maximum = Settings.Default.WhoAmITiming * 60;
            _timer.IsEnabled = false;
        }

        public Visibility WhoAmIHomeVisibility
        {
            get { return _whoAmIHomeVisibility; }
            set
            {
                _whoAmIHomeVisibility = value;
                OnPropertyChanged("WhoAmIHomeVisibility");
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
                CurrentSelect.Text = string.Format("{0}/{1}", _currentIndex + 1, _gameItems.Count);
                OnPropertyChanged("SelectedGameItem");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void LoadData()
        {
            var gameItems =
                FileManagement.GetListGameData().Where(x => x.GameName == Constant.GameConstant.WhoAmI).ToList();
            if (gameItems.Count > 0)
            {
                CreateSelectTopicButtons(gameItems.Select(x => x.Topic).Distinct());
            }
        }

        private void CreateSelectTopicButtons(IEnumerable<string> topics)
        {
            foreach (string topic in topics)
            {
                var viewbox = new Viewbox { Child = new TextBlock { Text = topic } };

                var button = new Button
                {
                    Content = viewbox,
                    Tag = topic,
                    Width = 500,
                    Height = 90,
                    Margin = new Thickness(0, 0, 10, 10)
                };

                button.Click += (s, e) =>
                {
                    var b = s as Button;
                    if (b != null)
                    {
                        WhoAmIHomeVisibility = Visibility.Collapsed;
                        _gameItems =EnglishGameHelper.RandomOrder(
                            FileManagement.GetListGameData().Where(
                                x => x.GameName == Constant.GameConstant.WhoAmI && x.Topic == b.Tag.ToString()).ToList());
                        SelectedGameItem = _gameItems[0];
                        _currentIndex = 0;
                        _timer.IsEnabled = true;
                    }
                };

                LayoutRoot.Children.Add(button);
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
            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 1, 0) }; //1s
            _timer.Tick += (o, s) =>
                               {
                                   EnglishGameHelper.SpeakNumberBetween((int)(TimingProgress.Maximum - ProgressBarValue));
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

        private void BackTopicClick(object sender, RoutedEventArgs e)
        {
            ProgressBarValue = 0;
            WhoAmIHomeVisibility = Visibility.Visible;
        }

        private void CopyAllWordClick(object sender, RoutedEventArgs e)
        {
            var wordList = string.Join(";", _gameItems.Select(x => x.Title).ToArray());
            Clipboard.SetText(wordList);
        }

        public void Dispose()
        {
            _timer.IsEnabled = false;
            _timer = null;
        }
    }
}