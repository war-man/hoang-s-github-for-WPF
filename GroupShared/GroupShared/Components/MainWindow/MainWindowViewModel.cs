using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GroupShared.Components.UserList;

namespace GroupShared.Components.MainWindow
{
    public class MainWindowViewModel
    {
        public PaymentViewModel PaymentViewModel { get; set; }
        public UserListViewModel UserListViewModel { get; set; }

        public MainWindowViewModel()
        {
            PaymentViewModel = new PaymentViewModel();
            UserListViewModel = new UserListViewModel();
            //Share load data
            PaymentViewModel.ReloadPartner = UserListViewModel.LoadData;
            UserListViewModel.ReloadPartner = PaymentViewModel.LoadData;
        }
    }
}
