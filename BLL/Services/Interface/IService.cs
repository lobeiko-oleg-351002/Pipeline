﻿using BllEntities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Services.Interface
{
    public interface IService<TEntity>
           where TEntity : IBllEntity
    {
        IEnumerable<TEntity> GetAll();

        TEntity Get(int id);

        void Create(TEntity entity);

        void Delete(int id);

        void Update(TEntity entity);

    }
}