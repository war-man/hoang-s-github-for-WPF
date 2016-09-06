using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;

namespace ClassEnglishGame
{
    public enum ActionState
    {
        None,
        Add,
        Update,
        Delete
    }

    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : INotifyPropertyChanged
    {
        #region Fields

        private ActionState _currentAction;
        private List<GameItem> _gameItems;
        private GameItem _newGameItem;
        private GameItem _selectedGameItem;
        private DispatcherTimer _timer;
        private GameItem _updateGameItem;
        private Visibility _canCloneAndDeleteVisibility;

        #endregion

        #region Constructor

        public Setting()
        {
            InitializeComponent();
            CreateTimer();
            DataContext = this;
            _currentAction = ActionState.None;

            //IntializeGroupData();

            LoadData();
            LoadAppSetting();
        }

        private void IntializeGroupData()
        {
            _isRaseSelectionChange = false;
            GameItemsView = new ListCollectionView(GameItems);
            if (GameItemsView.GroupDescriptions != null)
            {
                GroupApplyChange(GroupByGameNameCheckBox.IsChecked, "GameName");
                GroupApplyChange(GroupByTopicCheckBox.IsChecked, "Topic");
            }
            _isRaseSelectionChange = true;
        }

        private Visibility GetCloneAndDeleteVisibility()
        {
            return GameItems != null && GameItems.Count > 0 && _currentAction != ActionState.Add
                       ? Visibility.Visible
                       : Visibility.Collapsed;
        }

        #endregion

        #region Properties

        public List<GameItem> GameItems
        {
            get { return _gameItems; }
            set
            {
                _gameItems = value;
                OnPropertyChanged("GameItems");
            }
        }

        public ListCollectionView GameItemsView
        {
            get { return _gameItemsView; }
            set
            {
                _gameItemsView = value;
                OnPropertyChanged("GameItemsView");
            }
        }

        public GameItem NewGameItem
        {
            get { return _newGameItem; }
            set
            {
                _newGameItem = value;
                OnPropertyChanged("NewGameItem");
            }
        }

        public GameItem UpdateGameItem
        {
            get { return _updateGameItem; }
            set
            {
                _updateGameItem = value;
                OnPropertyChanged("UpdateGameItem");
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

        public Visibility CanCloneAndDeleteVisibility
        {
            get { return _canCloneAndDeleteVisibility; }
            set
            {
                _canCloneAndDeleteVisibility = value;
                OnPropertyChanged("CanCloneAndDeleteVisibility");
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

        #region Game Items

        private void LoadData()
        {
            //Backup update item
            var backup = new GameItem();
            if (_currentAction == ActionState.Update) backup = EnglishGameHelper.CloneItem(UpdateGameItem);
            //Get new list items
            GameItems = FileManagement.GetListGameData();
            IntializeGroupData();
            DataGridGameItem.UpdateLayout();

            //Show data
            if (GameItems.Count > 0)
            {
                switch (_currentAction)
                {
                    case ActionState.Add:
                        SelectedGameItem = GameItems.First(x => x.Title == NewGameItem.Title && x.GameName == NewGameItem.GameName);
                        break;
                    case ActionState.Update:
                        SelectedGameItem = GameItems.First(x => x.Title == backup.Title && x.GameName == backup.GameName);
                        break;
                    default:
                        SelectedGameItem = GameItems[0];
                        break;
                }
                _currentAction = ActionState.Update;
                EditorTitle.Text = "Selected item detail";
                ViewDetailItem.DataContext = UpdateGameItem;
            }
            else
            {
                AddNewGameItemClick(null, null);
            }

            CanCloneAndDeleteVisibility = GetCloneAndDeleteVisibility();
        }

        private void RaiseInformMessage(string message)
        {
            InformMessage.Text = message;
            InformMessage.Visibility = Visibility.Visible;
            _timer.IsEnabled = true;
        }

        private void CreateTimer()
        {
            if (_timer != null) return;
            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 3, 0) }; //3s
            _timer.Tick += (o, s) => { InformMessage.Visibility = Visibility.Collapsed; };
        }

        #endregion

        #region Events

        private void SelectImageClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
                             {
                                 Filter = "All file|*.*|JPEG file (.jpg)|*.jpg|PNG file (.png)|*.png|Bitmap file (.bmp)|*.bmp",
                                 Multiselect = false
                             };
            if (dialog.ShowDialog() != true) return;

            switch (_currentAction)
            {
                case ActionState.Update:
                    UpdateGameItem.Image = ConvertHelper.ImageToByte(dialog.FileName);
                    break;
                case ActionState.Add:
                    NewGameItem.Image = ConvertHelper.ImageToByte(dialog.FileName);
                    break;
            }

            PrevewPicture.Source = new BitmapImage(new Uri(dialog.FileName));
        }

        private void AddNewGameItemClick(object sender, RoutedEventArgs e)
        {
            _isRaseSelectionChange = false;
            DataGridGameItem.SelectedItem = null;
            _isRaseSelectionChange = true;

            NewGameItem = new GameItem();
            ViewDetailItem.DataContext = NewGameItem;
            _currentAction = ActionState.Add;
            EditorTitle.Text = "Add new item";
            CanCloneAndDeleteVisibility = GetCloneAndDeleteVisibility();
        }

        private void CloneGameItemClick(object sender, RoutedEventArgs e)
        {
            NewGameItem = EnglishGameHelper.CloneItem(SelectedGameItem);
            ViewDetailItem.DataContext = NewGameItem;
            _currentAction = ActionState.Add;
            EditorTitle.Text = "Add new item";
            CanCloneAndDeleteVisibility = GetCloneAndDeleteVisibility();

            _isRaseSelectionChange = false;
            DataGridGameItem.SelectedItem = null;
            _isRaseSelectionChange = true;
        }

        private bool _isRaseSelectionChange = true;

        private void ListItemSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (!_isRaseSelectionChange) return;

            UpdateGameItem = EnglishGameHelper.CloneItem(SelectedGameItem);
            ViewDetailItem.DataContext = UpdateGameItem;
            _currentAction = ActionState.Update;
            EditorTitle.Text = "Selected item detail";
            CanCloneAndDeleteVisibility = GetCloneAndDeleteVisibility();
        }

        private void DeleteGameItemClick(object sender, RoutedEventArgs e)
        {
            if (GameItems == null || GameItems.Count < 1 || SelectedGameItem == null)
            {
                MessageBox.Show("No selected item to delete", "Delete", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure to delete selected item?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                GameItems.Remove(SelectedGameItem);
                FileManagement.SaveListGameData(GameItems);
                //Re-load data
                LoadData();
            }
        }

        private void SaveGaveItemClick(object sender, RoutedEventArgs e)
        {
            //Validate data
            string message;
            if (!IsValidate(out message))
            {
                RaiseInformMessage(message);
                return;
            }

            // Add new or update
            switch (_currentAction)
            {
                case ActionState.Add:
                    GameItems.Add(NewGameItem);
                    FileManagement.SaveListGameData(GameItems);
                    RaiseInformMessage("Data has been added successfully");
                    break;
                case ActionState.Update:
                    UpdateProperty(SelectedGameItem, UpdateGameItem);
                    FileManagement.SaveListGameData(GameItems);
                    RaiseInformMessage("Data has been saved successfully");
                    break;
            }

            //Re-load data
            LoadData();
        }

        private bool IsValidate(out string message)
        {
            message = string.Empty;
            var result = true;
            var gameItem = _currentAction == ActionState.Add ? NewGameItem : UpdateGameItem;

            if (string.IsNullOrWhiteSpace(gameItem.GameName))
            {
                message = "You must select game";
                result = false;
            }
            else if (string.IsNullOrWhiteSpace(gameItem.Title))
            {
                message = "You must enter text";
                result = false;
            }
            //Todo: Validate for each game

            return result;
        }

        private void BackupGameItemClick(object sender, RoutedEventArgs e)
        {
            RaiseInformMessage("Data has been backup successfully in " + FileManagement.SaveBackupData(GameItems));
        }

        private void UpdateProperty(GameItem destination, GameItem source)
        {
            destination.GameName = source.GameName;
            destination.Topic = source.Topic;
            destination.Title = source.Title;
            destination.Image = source.Image;
            destination.IgnoreWord = source.IgnoreWord;
            destination.ExplainText = source.ExplainText;
        }

        private void SaveSettingClick(object sender, RoutedEventArgs e)
        {
            SaveAppSetting();
        }

        private void ImportListAndAddClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "All file|*.*|JPEG file (.jpg)|*.jpg|PNG file (.png)|*.png|Bitmap file (.bmp)|*.bmp",
                Multiselect = true
            };
            if (dialog.ShowDialog() != true) return;


            bool addQuestionMark = NewGameItem.GameName == Constant.GameConstant.TalkInMinute;

            foreach (var fileName in dialog.FileNames)
            {
                //NewGameItem
                GameItems.Add(new GameItem
                                  {
                                      GameName = NewGameItem.GameName,
                                      Topic = NewGameItem.Topic,
                                      Title = Path.GetFileNameWithoutExtension(fileName) + (addQuestionMark ? "?" : string.Empty),
                                      Image = ConvertHelper.ImageToByte(fileName)
                                  });
            }

            FileManagement.SaveListGameData(GameItems);
            RaiseInformMessage("Data has been imported successfully");
            //Re-load data
            NewGameItem.Title = Path.GetFileNameWithoutExtension(dialog.FileNames[0]) + (addQuestionMark ? "?" : string.Empty);
            LoadData();
        }

        #endregion

        #region App settings

        private int _talkInMinuteTiming;
        private int _tabooTiming;
        private int _charadesTiming;
        private int _pictureDashTiming;
        private int _whoAmITiming;
        private ListCollectionView _gameItemsView;

        public int TalkInMinuteTiming
        {
            get { return _talkInMinuteTiming; }
            set
            {
                _talkInMinuteTiming = value;
                OnPropertyChanged("TalkInMinuteTiming");
            }
        }

        public int TabooTiming
        {
            get { return _tabooTiming; }
            set
            {
                _tabooTiming = value;
                OnPropertyChanged("TabooTiming");
            }
        }

        public int CharadesTiming
        {
            get { return _charadesTiming; }
            set
            {
                _charadesTiming = value;
                OnPropertyChanged("CharadesTiming");
            }
        }

        public int PictureDashTiming
        {
            get { return _pictureDashTiming; }
            set
            {
                _pictureDashTiming = value;
                OnPropertyChanged("PictureDashTiming");
            }
        }

        public int WhoAmITiming
        {
            get { return _whoAmITiming; }
            set
            {
                _whoAmITiming = value;
                OnPropertyChanged("WhoAmITiming");
            }
        }

        private void LoadAppSetting()
        {
            TalkInMinuteTiming = Properties.Settings.Default.TalkInMinuteTiming;
            TabooTiming = Properties.Settings.Default.TabooTiming;
            CharadesTiming = Properties.Settings.Default.CharadesTiming;
            PictureDashTiming = Properties.Settings.Default.PictureDashTiming;
            WhoAmITiming = Properties.Settings.Default.WhoAmITiming;
        }

        private void SaveAppSetting()
        {
            Properties.Settings.Default.TalkInMinuteTiming = TalkInMinuteTiming;
            Properties.Settings.Default.TabooTiming = TabooTiming;
            Properties.Settings.Default.CharadesTiming = CharadesTiming;
            Properties.Settings.Default.PictureDashTiming = PictureDashTiming;
            Properties.Settings.Default.WhoAmITiming = WhoAmITiming;

            Properties.Settings.Default.Save();
        }

        #endregion

        #region Data Grid Group By

        private void GroupByGameNameCheckedChange(object sender, RoutedEventArgs e)
        {
            GroupApplyChange(((CheckBox)sender).IsChecked, "GameName");
        }

        private void GroupByTopicCheckedChange(object sender, RoutedEventArgs e)
        {
            GroupApplyChange(((CheckBox)sender).IsChecked, "Topic");
        }

        private void GroupApplyChange(bool? isChecked, string propertyName)
        {
            if (GameItemsView != null && GameItemsView.GroupDescriptions != null)
            {
                if (isChecked == true &&
                    GameItemsView.GroupDescriptions.All(x => ((PropertyGroupDescription)x).PropertyName != propertyName))
                {
                    GameItemsView.GroupDescriptions.Add(new PropertyGroupDescription(propertyName));
                }
                else if (isChecked == false &&
                    GameItemsView.GroupDescriptions.Any(x => ((PropertyGroupDescription)x).PropertyName == propertyName))
                {
                    GameItemsView.GroupDescriptions.Remove(
                        GameItemsView.GroupDescriptions.First(
                            x => ((PropertyGroupDescription)x).PropertyName == propertyName));
                }
                DataGridGameItem.UpdateLayout();
            }
        }

        #endregion
    }
}