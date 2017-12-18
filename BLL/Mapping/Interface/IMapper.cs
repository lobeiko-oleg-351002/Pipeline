using BllEntities.Interface;
using DAL.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Mapping.Interface
{
    public interface IMapper<TEntity, UEntity>
        where TEntity : IBllEntity
        where UEntity : IDalEntity
    {
        TEntity MapToBll(UEntity entity);
        UEntity MapToDal(TEntity entity);
    }
}
