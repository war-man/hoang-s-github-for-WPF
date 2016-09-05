using System;
using GroupShared.Business.Entities;

namespace GroupShared.Business
{
    public class UserBusiness : BaseBusiness<User>
    {
        public override void UpdateProperties(User source, User destination)
        {
            destination.Name = source.Name;
            destination.Updated = DateTime.Now;
        }
    }
}