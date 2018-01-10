using BllEntities.Interface;
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

        TEntity Create(TEntity entity);

        void Delete(int id);

        TEntity Update(TEntity entity);

    }
}
