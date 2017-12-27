using BllEntities.Interface;
using DAL.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Mapping.Interface
{
    public interface IEntitySimpleLibMapper<UEntity, TEntity> : IMapper<TEntity, IDalEntityLib>
        where UEntity : IBllEntity
        where TEntity : IBllEntitySimpleLib<UEntity>
    {

    }
}
