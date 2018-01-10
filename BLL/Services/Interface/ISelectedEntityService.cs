using BllEntities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Services.Interface
{
    public interface ISelectedEntityService<UEntity> : IService<IBllSelectedEntity<UEntity>>
            where UEntity : IBllEntity
    {
       
    }
}
