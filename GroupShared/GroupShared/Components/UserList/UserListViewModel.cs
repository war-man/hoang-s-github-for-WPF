using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GroupShared.Business;
using GroupShared.Business.Entities;
using GroupShared.Components.Base;
using Microsoft.Practices.Prism.Commands;

namespace GroupShared.Components.UserList
{
    public class UserListViewModel : BaseViewModel
    {
        private List<UserModel> _models;
        private UserModel _selectedItem;
        private UserModel _newItem;
        private Visibility _addNewVisibility;

        public List<UserModel> Models
        {
            get { return _models; }
            set
            {
                _models = value;
                OnPropertyChanged();
            }
        }

        public UserModel NewItem
        {
            get { return _newItem; }
            set
            {
                _newItem = value;
                OnPropertyChanged();
                AddPaymentCommand?.RaiseCanExecuteChanged();
            }
        }

        public UserModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                UpdatePaymentCommand?.RaiseCanExecuteChanged();
                DeletePaymentCommand?.RaiseCanExecuteChanged();
            }
        }

        public Visibility AddNewVisibility
        {
            get { return _addNewVisibility; }
            set
            {
                _addNewVisibility = value;
                OnPropertyChanged();
            }
        }

        public UserBusiness UserBusiness { get; set; }
        public PaymentBusiness PaymentBusiness { get; set; }

        public UserListViewModel()
        {
            UserBusiness = new UserBusiness();
            PaymentBusiness = new PaymentBusiness();
            AddNewVisibility = Visibility.Collapsed;

            InitCommands();
            LoadData(Guid.Empty);
            LoadPartner();
        }

        public void LoadData(Guid selectId)
        {
            var users = UserBusiness.GetList();
            var payments = PaymentBusiness.GetList();
            Models = CreateUsersModel(users, payments);

            if (Models.Count > 0)
            {
                var model = Models.FirstOrDefault(x => x.Id == selectId);
                SelectedItem = model ?? Models[0];
            }
            else
            {
                AddNewVisibility = Visibility.Visible;
            }

            //Create new item
            NewItem = new UserModel();
            NewItem.Id = Guid.NewGuid();
        }

        private void InitCommands()
        {
            NewPaymentCommand = new DelegateCommand(NewPaymentCommandExecute);
            UpdatePaymentCommand = new DelegateCommand(UpdatePaymentCommandExecute, () => SelectedItem != null);
            DeletePaymentCommand = new DelegateCommand(DeletePaymentCommandExecute, () => SelectedItem != null);
            AddPaymentCommand = new DelegateCommand(AddPaymentCommandExecute, () => NewItem != null);
            CancelPaymentCommand = new DelegateCommand(CancelPaymentCommandExecute);
        }

        public delegate void ReloadPartnerDelegate(Guid id);

        public ReloadPartnerDelegate ReloadPartner;

        private void LoadPartner()
        {
            ReloadPartner?.Invoke(Guid.Empty);
        }

        #region Commands

        public DelegateCommand NewPaymentCommand { get; set; }
        public DelegateCommand UpdatePaymentCommand { get; set; }
        public DelegateCommand DeletePaymentCommand { get; set; }
        public DelegateCommand AddPaymentCommand { get; set; }
        public DelegateCommand CancelPaymentCommand { get; set; }

        private void NewPaymentCommandExecute()
        {
            AddNewVisibility = Visibility.Visible;
        }

        private void UpdatePaymentCommandExecute()
        {
            if (!IsValidItem(SelectedItem))
            {
                MessageBox.Show("Invalid input!", "Group Shared", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UserBusiness.Save(ConvertToUser(SelectedItem));
            //Reload data
            LoadData(SelectedItem.Id);
            LoadPartner();
        }

        private void DeletePaymentCommandExecute()
        {
            //Confirm?
            if (MessageBoxResult.Yes == MessageBox.Show("Do you want to delete selected user?", "Group Shared", MessageBoxButton.YesNo, MessageBoxImage.Warning))
            {
                //Delete
                UserBusiness.Delete(SelectedItem.Id);
                //Reload data
                LoadData(Guid.Empty);
                LoadPartner();
            }
        }

        private void AddPaymentCommandExecute()
        {
            if (!IsValidItem(NewItem))
            {
                MessageBox.Show("Invalid input!", "Group Shared", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            UserBusiness.Save(ConvertToUser(NewItem));
            AddNewVisibility = Visibility.Collapsed;
            //Reload data
            LoadData(NewItem.Id);
            LoadPartner();
        }

        private void CancelPaymentCommandExecute()
        {
            AddNewVisibility = Visibility.Collapsed;
        }

        #endregion

        #region Helpers



        private User ConvertToUser(UserModel item)
        {
            return new User
            {
                Id = item.Id,
                Name = item.Name,
            };
        }

        private bool IsValidItem(UserModel item)
        {
            var isValid = item != null && item.Id != Guid.Empty && !string.IsNullOrEmpty(item.Name);
            return isValid;
        }

        private List<UserModel> CreateUsersModel(List<User> users, List<Payment> payments)
        {
            foreach (User user in users)
            {
                user.Spent = 0;
            }

            foreach (var payment in payments)
            {
                var userIds = payment.Users.Select(x => x.Id).ToList();
                foreach (User user in users)
                {
                    if (userIds.Contains(user.Id))
                    {
                        user.Spent += payment.Users.First(x => x.Id == user.Id).Spent;
                    }
                }
            }

            return users.Select(x => new UserModel
            {
                Id = x.Id,
                Name = x.Name,
                TotalSpent = x.Spent
            }).ToList();
        }

        #endregion
    }
}
