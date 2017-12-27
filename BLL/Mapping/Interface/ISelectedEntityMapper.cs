using BllEntities.Interface;
using DAL.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Mapping.Interface
{
    public interface ISelectedEntityMapper<UEntity> : IMapper<IBllSelectedEntity<UEntity>, IDalSelectedEntity>
        where UEntity : IBllEntity
    {
        new IBllSelectedEntity<UEntity> MapToBll(IDalSelectedEntity entity);
        new IDalSelectedEntity MapToDal(IBllSelectedEntity<UEntity> entity);
    }
}
