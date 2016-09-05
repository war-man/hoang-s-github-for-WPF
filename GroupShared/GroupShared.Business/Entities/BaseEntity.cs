using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupShared.Business.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid ModifiedBy { get; set; }
    }
}
