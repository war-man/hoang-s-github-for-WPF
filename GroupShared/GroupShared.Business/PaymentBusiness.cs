using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GroupShared.Business.Entities;
using Newtonsoft.Json;

namespace GroupShared.Business
{
    public class PaymentBusiness : BaseBusiness<Payment>
    {
        public override void UpdateProperties(Payment source, Payment destination)
        {
            destination.Date = source.Date;
            destination.Spent = source.Spent;
            destination.Reason = source.Reason;
            destination.Updated = DateTime.Now;
            destination.Users = source.Users;
        }

        public void Baseline()
        {
            string today = DateTime.Today.ToString("yyyMMdd");

            var userPathBaseline = Path.Combine(ConfigHelper.BaselineFolder,
                string.Format(ConfigHelper.BaselineUserFile, today));
            File.Copy(ConfigHelper.UserFile, userPathBaseline, true);

            var paymentPathBaseline = Path.Combine(ConfigHelper.BaselineFolder,
                string.Format(ConfigHelper.BaselinePaymentFile, today));
            File.Copy(ConfigHelper.PaymentFile, paymentPathBaseline, true);

            //Delete current file
            File.Delete(ConfigHelper.PaymentFile);
        }

        public void Export(string filePaymentName = null)
        {
            //Get export file
            string fileUserName;
            if (string.IsNullOrEmpty(filePaymentName) || !File.Exists(filePaymentName))
            {
                filePaymentName = ConfigHelper.PaymentFile;
                fileUserName = ConfigHelper.UserFile;
            }
            else
            {
                fileUserName = filePaymentName.Replace("_payments", "_users");
                if (!File.Exists(fileUserName))
                {
                    fileUserName = ConfigHelper.UserFile;
                }
            }

            //Get export data
            var json = File.ReadAllText(fileUserName);
            var listUsers = JsonConvert.DeserializeObject<List<User>>(json);
            var listPayments = GetList(filePaymentName);

            var content = new StringBuilder();

            //Header
            string header = "Date,Spent,Reason";
            foreach (User user in listUsers)
            {
                header += "," + user.Name;
            }

            content.AppendLine(header);

            //Content
            foreach (Payment payment in listPayments)
            {
                var line = string.Format("\"{0}\",\"{1:C}\",\"{2}\"", payment.Date.ToString("yyyy MMMM dd"),
                    payment.Spent, payment.Reason);
                //var avg = payment.Spent/payment.Users.Length;
                var userIds = payment.Users.Select(x => x.Id).ToList();
                foreach (User user in listUsers)
                {
                    if (userIds.Contains(user.Id))
                    {
                        var sp = payment.Users.First(x => x.Id == user.Id).Spent;
                        line += "," + $"{sp:C}";
                    }
                    else
                    {
                        line += ",$0.00";
                    }
                }

                content.AppendLine(line);
            }

            //Save file
            string today = DateTime.Today.ToString("yyyMMdd");
            var exportPath = Path.Combine(ConfigHelper.ExportFolder, string.Format(ConfigHelper.ExportPaymentFile, today));
            File.WriteAllText(exportPath, content.ToString());

            ExportUserBalance(listUsers, listPayments);
        }

        private void ExportUserBalance(List<User> users, List<Payment> payments)
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

            var content = new StringBuilder();
            content.AppendLine("Name,Spent");
            foreach (User user in users)
            {
                content.AppendLine(string.Format("{0},{1:C}", user.Name, user.Spent));
            }

            //Save file
            string today = DateTime.Today.ToString("yyyMMdd");
            var exportPath = Path.Combine(ConfigHelper.ExportFolder, string.Format(ConfigHelper.ExportUserFile, today));
            File.WriteAllText(exportPath, content.ToString());
        }
    }
}