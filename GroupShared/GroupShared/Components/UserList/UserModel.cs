using System;
using GroupShared.Commons;
using GroupShared.Components.Base;

namespace GroupShared.Components.UserList
{
    public class UserModel : BaseModel
    {
        public string Name { get; set; }
        public bool IsSpent { get; set; }
        public decimal Spent { get; set; }
        public string SpentText => $"{Spent:C}";
        public decimal TotalSpent { get; set; }

        public string TotalSpentText => $"{TotalSpent:C}";

        public UserModel Clone()
        {
            var item = Utils.Clone(this);
            item.Id = Guid.NewGuid();
            return item;
        }
    }
}