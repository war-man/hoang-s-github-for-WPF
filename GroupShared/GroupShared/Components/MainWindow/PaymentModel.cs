using System;
using System.Collections.ObjectModel;
using GroupShared.Commons;
using GroupShared.Components.Base;
using GroupShared.Components.UserList;

namespace GroupShared.Components.MainWindow
{
    public class PaymentModel : BaseModel
    {
        private DateTime _date;
        private decimal _spent;
        private string _reason;
        private ObservableCollection<UserModel> _users;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        public string DateText => Date.ToString("yyyy MMMM dd");

        public decimal Spent
        {
            get { return _spent; }
            set
            {
                _spent = value;
                OnPropertyChanged();
            }
        }

        public string SpentText => $"{Spent:C}";

        public string Reason
        {
            get { return _reason; }
            set
            {
                _reason = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<UserModel> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        public UserModel SelectedUser { get; set; }

        public PaymentModel Clone()
        {
            var item = Utils.Clone(this);
            item.Id = Guid.NewGuid();
            return item;
        }
    }
}