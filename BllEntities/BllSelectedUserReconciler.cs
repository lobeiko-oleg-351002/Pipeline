using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities
{
    public class BllSelectedUserReconciler : BllSelectedEntity<BllUser>
    {
        public bool? IsEventReconciled { get; set; }
    }
}
