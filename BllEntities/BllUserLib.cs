using BllEntities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities
{
    public class BllUserLib : IBllEntityLib<BllUser>
    {
        public int Id { get; set; }

        public List<BllSelectedEntity<BllUser>> SelectedEntities { get; set; }

        public BllUserLib()
        {
            SelectedEntities = new List<BllSelectedEntity<BllUser>>();
        }
    }
}
