﻿using BllEntities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Services.Interface
{
    public interface IEntityLibService<UEntity, TEntity> : IService<TEntity>
        where UEntity : IBllEntity
        where TEntity : IBllEntityLib<UEntity>
    {
        new TEntity Create(TEntity entity);
        new TEntity Update(TEntity entity);
    }
}
