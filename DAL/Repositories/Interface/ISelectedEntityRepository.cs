﻿using DAL.Entities.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories.Interface
{
    public interface ISelectedEntityRepository<UEntity> : IRepository<IDalSelectedEntity, UEntity>
        where UEntity : class, ISelectedEntity
    {
        IEnumerable<IDalSelectedEntity> GetEntitiesByLibId(int id);

    }
}
