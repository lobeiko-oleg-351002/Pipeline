using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Entities
{
    public class DalSelectedUser : DalSelectedEntity
    {
        public bool IsEventAccepted { get; set; }
    }
}
