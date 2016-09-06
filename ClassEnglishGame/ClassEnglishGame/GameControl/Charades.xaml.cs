using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Threading;
using ClassEnglishGame.Properties;

namespace ClassEnglishGame.GameControl
{
    /// <summary>
    /// Interaction logic for Charades.xaml
    /// </summary>
    public partial class Charades : INotifyPropertyChanged
    {
        private int _currentIndex;
        private List<GameItem> _gameItems;
        private int _progressBarValue;
        private GameItem _selectedGameItem;
        private Visibility _charadesHomeVisibility;
        private DispatcherTimer _timer;

        public Charades()
        {
            InitializeComponent();
            DataContext = this;
            LoadData();
            CharadesHomeVisibility = Visibility.Visible;
            CreateTimer();
            ProgressBarValue = 0;
            TimingProgress.Maximum = Settings.Default.CharadesTiming*60;
            _timer.IsEnabled = false;
        }

        public Visibility CharadesHomeVisibility
        {
            get { return _charadesHomeVisibility; }
            set
            {
                _charadesHomeVisibility = value;
                OnPropertyChanged("CharadesHomeVisibility");
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
            _gameItems = EnglishGameHelper.RandomOrder(
                FileManagement.GetListGameData().Where(x => x.GameName == Constant.GameConstant.Charades).ToList());
            if (_gameItems != null && _gameItems.Count > 0)
            {
                SelectedGameItem = _gameItems[0];
                _currentIndex = 0;
            }
        }

        private void StartGameClick(object sender, RoutedEventArgs e)
        {
            if (_gameItems == null || _gameItems.Count < 1)
            {
                MessageBox.Show("Cannot find data of Charades game.", "Charades:", MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                CharadesHomeVisibility = Visibility.Visible;
            }
            else
            {
                CharadesHomeVisibility = Visibility.Collapsed;
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
            if (_currentIndex < _gameItems.Count - 1)
                _currentIndex++;
            else
                _currentIndex = 0;

            SelectedGameItem = _gameItems[_currentIndex];
            ProgressBarValue = 0;
        }

        private void CreateTimer()
        {
            if (_timer != null) return;
            _timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 1, 0)}; //1s
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

        private void CopyAllWordClick(object sender, RoutedEventArgs e)
        {
            var wordList = string.Join(";", _gameItems.Select(x => x.Title).ToArray());
            Clipboard.SetText(wordList);
        }
    }
}