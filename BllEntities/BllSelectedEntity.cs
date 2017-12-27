using BllEntities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities
{
    public class BllSelectedEntity<UEntity> : IBllSelectedEntity<UEntity>
        where UEntity : IBllEntity
    {
        public UEntity Entity { get; set; }
        public int Id { get; set; }
    }
}
