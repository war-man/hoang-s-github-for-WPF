using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ClassEnglishGame.GameControl
{
    /// <summary>
    /// Interaction logic for PictureDash.xaml
    /// </summary>
    public partial class PictureDash : INotifyPropertyChanged
    {
        private int _currentIndex;
        private List<GameItem> _gameItems;
        private Visibility _pictureDashHomeVisibility;
        private GameItem _selectedGameItem;
        private Visibility _showTitleVisibility;
        private Storyboard _storyboard;

        public PictureDash()
        {
            InitializeComponent();
            DataContext = this;
            CreateAnnimation();
            LoadData();
            PictureDashHomeVisibility = Visibility.Visible;
        }

        public Visibility PictureDashHomeVisibility
        {
            get { return _pictureDashHomeVisibility; }
            set
            {
                _pictureDashHomeVisibility = value;
                OnPropertyChanged("PictureDashHomeVisibility");
            }
        }

        public GameItem SelectedGameItem
        {
            get { return _selectedGameItem; }
            set
            {
                _selectedGameItem = value;
                CurrentSelect.Text = string.Format("{0}/{1}", _currentIndex + 1, _gameItems.Count);
                ShowTitleVisibility = Visibility.Collapsed;
                OnPropertyChanged("SelectedGameItem");
            }
        }

        public Visibility ShowTitleVisibility
        {
            get { return _showTitleVisibility; }
            set
            {
                _showTitleVisibility = value;
                OnPropertyChanged("ShowTitleVisibility");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void CreateAnnimation()
        {
            _storyboard = new Storyboard();

            var animation = new DoubleAnimation
                                {
                                    From = -1000,
                                    To = 2000,
                                    Duration = TimeSpan.FromSeconds(Properties.Settings.Default.PictureDashTiming)
                                };
            Storyboard.SetTarget(animation, MovingImage);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));

            _storyboard.Children.Add(animation);
        }

        private void LoadData()
        {
            List<GameItem> gameItems =
                FileManagement.GetListGameData().Where(x => x.GameName == Constant.GameConstant.PictureDash).ToList();
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
                                            PictureDashHomeVisibility = Visibility.Collapsed;
                                            _gameItems =EnglishGameHelper.RandomOrder(
                                                FileManagement.GetListGameData().Where(
                                                    x =>
                                                    x.GameName == Constant.GameConstant.PictureDash &&
                                                    x.Topic == b.Tag.ToString()).ToList());
                                            SelectedGameItem = _gameItems[0];
                                            _currentIndex = 0;
                                            _storyboard.Begin();
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
            if (_currentIndex > 0)
                _currentIndex--;
            else
                _currentIndex = _gameItems.Count - 1;

            SelectedGameItem = _gameItems[_currentIndex];
            _storyboard.Begin();
        }

        private void NextClick(object sender, RoutedEventArgs e)
        {
            if (_currentIndex < _gameItems.Count - 1)
                _currentIndex++;
            else
                _currentIndex = 0;

            SelectedGameItem = _gameItems[_currentIndex];
            _storyboard.Begin();
        }

        private void BackTopicClick(object sender, RoutedEventArgs e)
        {
            PictureDashHomeVisibility = Visibility.Visible;
        }

        private void ShowClick(object sender, RoutedEventArgs e)
        {
            ShowTitleVisibility = Visibility.Visible;
            _storyboard.Stop();
            MovingImage.SetValue(Canvas.LeftProperty, (ActualWidth - MovingImage.ActualWidth) / 2);
            EnglishGameHelper.SpeakText(SelectedGameItem.Title);
        }

        private void CopyAllWordClick(object sender, RoutedEventArgs e)
        {
            var wordList = string.Join(";", _gameItems.Select(x => x.Title).ToArray());
            Clipboard.SetText(wordList);
        }
    }
}