using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupShared.Business.Entities
{
   public class User: BaseEntity
    {
       public string Name { get; set; }
       public decimal Spent { get; set; }
    }
}
