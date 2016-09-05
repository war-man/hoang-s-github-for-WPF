using System;
using System.Collections.Generic;
using GroupShared.Business;
using GroupShared.Business.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GroupShared.Test
{
    [TestClass]
    public class UserBusinessTest
    {
        UserBusiness _userBusiness = new UserBusiness();


        [TestMethod]
        public void GetListTest()
        {
            var list = _userBusiness.GetList();
            Assert.AreEqual(6, list.Count);
        }

        [TestMethod]
        public void SaveListTest()
        {
            var now = DateTime.Now;
            var users = new List<User>
            {
                new User {Id = Guid.NewGuid(), Name = "Hoang", Created =now, Updated = now},
                new User {Id = Guid.NewGuid(), Name = "Ngoc", Created =now, Updated = now},
                new User {Id = Guid.NewGuid(), Name = "Phuong", Created =now, Updated = now},
                new User {Id = Guid.NewGuid(), Name = "Ha", Created =now, Updated = now},
                new User {Id = Guid.NewGuid(), Name = "Trung", Created =now, Updated = now},
                new User {Id = Guid.NewGuid(), Name = "Son", Created =now, Updated = now},
            };
            _userBusiness.SaveList(users);
        }
    }
}
