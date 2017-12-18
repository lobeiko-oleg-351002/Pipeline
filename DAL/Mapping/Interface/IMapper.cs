using DAL.Entities.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Mapping.Interface
{
    public interface IMapper<TEntity, UEntity>
        where TEntity : IDalEntity
        where UEntity : IOrmEntity
    {
        TEntity MapToDal(UEntity entity);
        UEntity MapToOrm(TEntity entity);
    }
}
