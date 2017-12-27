using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities.Interface
{
    public interface IBllEntitySimpleLib<UEntity> : IBllEntity
          where UEntity : IBllEntity
    {
        List<UEntity> Entities { get; set; }
    }
}
