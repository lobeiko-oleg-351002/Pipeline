using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities.Interface
{
    public interface IBllSelectedEntity<UEntity> : IBllEntity
        where UEntity : IBllEntity
    {
        UEntity Entity { get; set; }
    }
}
