using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities
{
    public class BllSelectedUser : BllSelectedEntity<BllUser>
    {
        public bool IsEventAccepted { get; set; }
    }
}
