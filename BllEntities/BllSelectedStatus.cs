using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities
{
    public class BllSelectedStatus : BllSelectedEntity<BllStatus>
    {
        public DateTime Date { get; set; }
    }
}
