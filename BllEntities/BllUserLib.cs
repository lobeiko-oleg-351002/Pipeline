using BllEntities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities
{
    public class BllUserLib : IBllEntity
    {
        public int Id { get; set; }

        public List<BllSelectedUser> SelectedEntities { get; set; }

        public BllUserLib()
        {
            SelectedEntities = new List<BllSelectedUser>();
        }
    }
}
