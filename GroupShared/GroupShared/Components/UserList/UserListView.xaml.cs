using System;
using System.Windows;

namespace GroupShared.Components.UserList
{
    /// <summary>
    ///     Interaction logic for UserListView.xaml
    /// </summary>
    public partial class UserListView : Window
    {
        public delegate void ReloadParentDelegate(Guid id);

        public ReloadParentDelegate ReloadParent;

        public UserListView()
        {
            InitializeComponent();
            Closed += (s, e) => { ReloadParent?.Invoke(Guid.Empty); };
        }
    }
}