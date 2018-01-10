using DAL.Entities.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories.Interface
{
    public interface IRepository<TEntity, UEntity>
            where TEntity : IDalEntity
            where UEntity : IOrmEntity
    {
        TEntity Create(TEntity entity);

        UEntity CreateAndReturnOrm(TEntity entity);

        IEnumerable<TEntity> GetAll();

        TEntity Get(int id);

        void Delete(int id);

        TEntity Update(TEntity entity);
    }
}
