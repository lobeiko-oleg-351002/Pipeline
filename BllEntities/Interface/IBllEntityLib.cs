using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities.Interface
{
    public interface IBllEntityLib<TEntity> : IBllEntity
        where TEntity : IBllEntity
    {
        List<BllSelectedEntity<TEntity>> SelectedEntities { get; set; }
    }
}
