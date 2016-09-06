using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ClassEnglishGame.Properties;

namespace ClassEnglishGame.GameControl
{
    /// <summary>
    /// Interaction logic for TalkInMinute.xaml
    /// </summary>
    public partial class TalkInMinute : INotifyPropertyChanged
    {
        #region Fields

        private int _progressBarValue;
        private Visibility _selectTopicVisibility;
        private GameItem _selectedGameItem;
        private DispatcherTimer _timer;

        #endregion

        public TalkInMinute()
        {
            InitializeComponent();
            DataContext = this;
            CreateSelectTopicButtons();
            SelectTopicVisibility = Visibility.Visible;
            CreateTimer();
            ProgressBarValue = 0;
            TimingProgress.Maximum = Settings.Default.TalkInMinuteTiming*60;
            _timer.IsEnabled = false;
        }

        #region Properties

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
                OnPropertyChanged("SelectedGameItem");
            }
        }

        public Visibility SelectTopicVisibility
        {
            get { return _selectTopicVisibility; }
            set
            {
                _selectTopicVisibility = value;
                OnPropertyChanged("SelectTopicVisibility");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

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
                                       new SoundPlayer(Properties.Resources.beep).Play();
                                   }
                               };
        }

        private void CreateSelectTopicButtons()
        {
            IEnumerable<GameItem> talkInMinuteItems =EnglishGameHelper.RandomOrder(
                FileManagement.GetListGameData().Where(x => x.GameName == Constant.GameConstant.TalkInMinute).ToList());
            int index = 1;
            foreach (GameItem talkInMinuteItem in talkInMinuteItems)
            {
                var viewbox = new Viewbox {Child = new TextBlock {Text = index.ToString(CultureInfo.InvariantCulture)}};

                var button = new Button
                                 {
                                     Content = viewbox,
                                     Tag = talkInMinuteItem,
                                     Width = 100,
                                     Height = 100,
                                     Margin = new Thickness(0, 0, 10, 10)
                                 };

                button.Click += (s, e) =>
                                    {
                                        var b = s as Button;
                                        if (b != null)
                                        {
                                            SelectTopicVisibility = Visibility.Collapsed;
                                            SelectedGameItem = (GameItem) b.Tag;
                                            b.Visibility = Visibility.Collapsed;
                                            EnglishGameHelper.SpeakText(SelectedGameItem.Title);
                                            _timer.IsEnabled = true;
                                        }
                                    };

                LayoutRoot.Children.Add(button);
                index++;
            }
        }

        private void BackToSelectTopicClick(object sender, RoutedEventArgs e)
        {
            ProgressBarValue = 0;
            _timer.IsEnabled = false;
            SelectTopicVisibility = Visibility.Visible;
        }
    }
}