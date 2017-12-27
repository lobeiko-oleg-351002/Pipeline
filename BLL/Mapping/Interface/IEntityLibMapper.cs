using BllEntities.Interface;
using DAL.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Mapping.Interface
{
    public interface IEntityLibMapper<UEntity, YEntity> : IMapper<YEntity, IDalEntityLib>
        where UEntity : IBllEntity
        where YEntity : IBllEntityLib<UEntity>

    {

    }
}
