using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace ClassEnglishGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigateManagement.InitNavigate(this);
            NavigateManagement.SetDefaultNavigate();

            SetMainControl(new Welcome());
        }

        private void NavigateToPageClick(object sender, RoutedEventArgs e)
        {
            NavigateManagement.NavigateToPage(((Button)sender).Tag.ToString());
        }

        public void SetMainControl(UserControl control, string title = "Welcome")
        {
            //Add control to main
            if (MainControl.Children.Count > 0)
            {
                MainControl.Children.Clear();
            }

            Title = "English Game - " + title;
            MainControl.Children.Add(control);
        }

        private void MainKeyUp(object sender, KeyEventArgs e)
        {
            //if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Alt) && e.Key == Key.L)
            if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.System)
            {
                SettingFunction.Visibility = Visibility.Visible;
            }
            else if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Escape)
            {
                SettingFunction.Visibility = Visibility.Collapsed;
            }
        }
    }
}