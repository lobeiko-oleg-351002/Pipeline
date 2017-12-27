using BllEntities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities
{
    public class BllStatusLib : IBllEntity
    {
        public int Id { get; set; }  

        public List<BllSelectedStatus> SelectedEntities { get; set; }

        public BllStatusLib()
        {
            SelectedEntities = new List<BllSelectedStatus>();
        }
    }
}
