using BllEntities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities
{
    public class BllEventTypeLib : IBllEntityLib<BllEventType>
    {
        public int Id { get; set; }

        public List<BllSelectedEntity<BllEventType>> SelectedEntities { get; set; }

        public BllEventTypeLib()
        {
            SelectedEntities = new List<BllSelectedEntity<BllEventType>>();
        }
    }
}
