using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using GroupShared.Business;
using GroupShared.Business.Entities;
using GroupShared.Components.Base;
using GroupShared.Components.UserList;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;

namespace GroupShared.Components.MainWindow
{
    public class PaymentViewModel : BaseViewModel
    {
        private Visibility _addNewVisibility;
        private List<PaymentModel> _models;
        private PaymentModel _newItem;

        private PaymentModel _selectedItem;

        public PaymentViewModel()
        {
            UserBusiness = new UserBusiness();
            PaymentBusiness = new PaymentBusiness();

            AddNewVisibility = Visibility.Collapsed;
           
            InitCommands();
            LoadData(Guid.Empty);
            LoadPartner();
        }

        public UserBusiness UserBusiness { get; set; }
        public PaymentBusiness PaymentBusiness { get; set; }

        public List<PaymentModel> Models
        {
            get { return _models; }
            set
            {
                _models = value;
                OnPropertyChanged();
            }
        }

        public PaymentModel SelectedItem
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

        public PaymentModel NewItem
        {
            get { return _newItem; }
            set
            {
                _newItem = value;
                OnPropertyChanged();

                AddPaymentCommand?.RaiseCanExecuteChanged();
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

        private void InitCommands()
        {
            NewPaymentCommand = new DelegateCommand(NewPaymentCommandExecute);
            UpdatePaymentCommand = new DelegateCommand(UpdatePaymentCommandExecute, () => SelectedItem != null);
            DeletePaymentCommand = new DelegateCommand(DeletePaymentCommandExecute, () => SelectedItem != null);
            AddPaymentCommand = new DelegateCommand(AddPaymentCommandExecute, () => NewItem != null);
            CancelPaymentCommand = new DelegateCommand(CancelPaymentCommandExecute);
            BaselineCommand = new DelegateCommand(BaselineCommandExecute);
            ExportCommand = new DelegateCommand(ExportCommandExecute);
            ExportBaselineCommand = new DelegateCommand(ExportBaselineCommandExecute);
            OpenUserListCommand = new DelegateCommand(OpenUserListCommandExecute);
        }

        public void LoadData(Guid selectId)
        {
            var fullUsers = UserBusiness.GetList();
            if (fullUsers == null || fullUsers.Count == 0)
            {
                
                //MessageBox.Show("Please input users to the app", "Group Shared", MessageBoxButton.OK,
                //    MessageBoxImage.Information);
                OpenUserListCommandExecute();
                return;
            }

            //Load payments
            var payments = PaymentBusiness.GetList();

            Models = CreatePaymentModel(payments, fullUsers);
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
            NewItem = new PaymentModel();
            NewItem.Id = Guid.NewGuid();
            NewItem.Date = DateTime.Today;
            NewItem.Users = new ObservableCollection<UserModel>(fullUsers.Select(x => new UserModel
            {
                Id = x.Id,
                Name = x.Name,
                IsSpent = false
            }));
        }

        public delegate void ReloadPartnerDelegate(Guid id);

        public ReloadPartnerDelegate ReloadPartner;

        private void LoadPartner()
        {
            ReloadPartner?.Invoke(Guid.Empty);
        }

        #region Validate

        private bool IsValidItem(PaymentModel model)
        {
            var isValid = model.Id != Guid.Empty && model.Spent > 0 && !string.IsNullOrEmpty(model.Reason) &&
                          model.Users != null && model.Users.Count > 0 && model.Users.Any(x => x.IsSpent);
            return isValid;
        }

        #endregion

        #region Commands

        public DelegateCommand NewPaymentCommand { get; set; }
        public DelegateCommand UpdatePaymentCommand { get; set; }
        public DelegateCommand DeletePaymentCommand { get; set; }
        public DelegateCommand AddPaymentCommand { get; set; }
        public DelegateCommand CancelPaymentCommand { get; set; }
        public DelegateCommand BaselineCommand { get; set; }
        public DelegateCommand ExportCommand { get; set; }
        public DelegateCommand ExportBaselineCommand { get; set; }
        public DelegateCommand OpenUserListCommand { get; set; }

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
            PaymentBusiness.Save(ConvertToPayment(SelectedItem));
            //Reload data
            LoadData(SelectedItem.Id);
            LoadPartner();
        }

        private void DeletePaymentCommandExecute()
        {
            //Confirm?
            if (MessageBoxResult.Yes ==
                MessageBox.Show("Do you want to delete selected payment?", "Group Shared", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning))
            {
                //Delete
                PaymentBusiness.Delete(SelectedItem.Id);
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
            PaymentBusiness.Save(ConvertToPayment(NewItem));
            AddNewVisibility = Visibility.Collapsed;
            //Reload data
            LoadData(NewItem.Id);
            LoadPartner();
        }

        private void CancelPaymentCommandExecute()
        {
            AddNewVisibility = Visibility.Collapsed;
        }

        private void BaselineCommandExecute()
        {
            if (MessageBoxResult.Yes ==
                MessageBox.Show("Baseline will delete all current payments. Are you sure want to baseline?",
                    "Group Shared", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning))
            {
                PaymentBusiness.Baseline();
                LoadData(Guid.Empty);
                LoadPartner();
            }
        }

        private void ExportCommandExecute()
        {
            PaymentBusiness.Export();
            //MessageBox.Show("Done!", "Group Shared");
            Process.Start(Path.Combine(Environment.CurrentDirectory, ConfigHelper.ExportFolder));
        }

        private void ExportBaselineCommandExecute()
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = Path.Combine(Environment.CurrentDirectory, ConfigHelper.BaselineFolder),
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                PaymentBusiness.Export(dialog.FileName);
                //MessageBox.Show("Done!", "Group Shared");
                Process.Start(Path.Combine(Environment.CurrentDirectory, ConfigHelper.ExportFolder));
            }
        }

        private void OpenUserListCommandExecute()
        {
            var dialog = new UserListView {ReloadParent = LoadData};
            dialog.ShowDialog();
        }

        #endregion

        #region Helper

        private List<PaymentModel> CreatePaymentModel(List<Payment> payments, List<User> fullUsers)
        {
            var list = new List<PaymentModel>();
            var i = 0;
            foreach (var payment in payments)
            {
                list.Add(new PaymentModel
                {
                    Id = payment.Id,
                    Date = payment.Date,
                    Spent = payment.Spent,
                    Reason = payment.Reason,
                    Users = new ObservableCollection<UserModel>()
                    //another fields
                });

                var userIds = payment.Users.Select(x => x.Id).ToArray();
                foreach (var user in fullUsers)
                {
                    list[i].Users.Add(
                        new UserModel
                        {
                            Id = user.Id,
                            Name = user.Name,
                            IsSpent = userIds.Contains(user.Id),
                            Spent = userIds.Contains(user.Id) ? payment.Users.First(x => x.Id == user.Id).Spent : 0
                        });
                }

                i++;
            }

            return list;
        }

        private Payment ConvertToPayment(PaymentModel item)
        {
            //Reset spent
            foreach (UserModel user in item.Users)
            {
                user.Spent = 0;
            }

            //Calculate spent
            var count = item.Users.Count(x => x.IsSpent);
            var avg = item.Spent/count;
            foreach (var user in item.Users.Where(x => x.IsSpent))
            {
                user.Spent = Math.Round(avg, 2);
            }

            UserModel balanceUser;
            if (item.SelectedUser != null)
            {
                balanceUser = item.Users.FirstOrDefault(x => x.Id == item.SelectedUser.Id) ?? item.Users[0];
            }
            else
            {
                balanceUser = item.Users[0];
            }

            balanceUser.Spent += (item.Spent*-1);
            balanceUser.IsSpent = true;

            return new Payment
            {
                Id = item.Id,
                Date = item.Date,
                Spent = item.Spent,
                Reason = item.Reason,
                Users =
                    item.Users.Where(x => x.IsSpent)
                        .Select(x => new User {Id = x.Id, Name = x.Name, Spent = x.Spent})
                        .ToArray()
            };
        }

        #endregion
    }
}