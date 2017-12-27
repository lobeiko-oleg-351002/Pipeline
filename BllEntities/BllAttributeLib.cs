using BllEntities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities
{
    public class BllAttributeLib : IBllEntityLib<BllAttribute>
    {
        public int Id { get; set; }

        public List<BllSelectedEntity<BllAttribute>> SelectedEntities { get; set; }

        public BllAttributeLib()
        {
            SelectedEntities = new List<BllSelectedEntity<BllAttribute>>();
        }
    }
}
