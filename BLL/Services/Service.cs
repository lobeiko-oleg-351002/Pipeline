using BLL.Mapping.Interface;
using BLL.Services.Interface;
using BllEntities.Interface;
using DAL.Entities.Interface;
using DAL.Repositories.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Services
{
    public class Service<T, U, Y, Mapper> : IService<T>
            where T : class, IBllEntity
            where U : class, IDalEntity
            where Y : class, IOrmEntity
            where Mapper : class, IMapper<T, U>, new()
    {
        protected readonly IUnitOfWork uow;
        protected readonly IRepository<U, Y> repository;
        protected Mapper mapper;
        public Service(IUnitOfWork uow, IRepository<U, Y> repository)
        {
            this.uow = uow;
            this.repository = repository;
            InitMapper();
        }

        protected virtual void InitMapper()
        {
            mapper = new Mapper();
        }

        public virtual T Create(T entity)
        {
            var newEntity = repository.Create(mapper.MapToDal(entity));
            uow.Commit();
            return mapper.MapToBll(newEntity);
        }

        public virtual void Delete(int id)
        {
            repository.Delete(id);
            uow.Commit();
        }

        public virtual IEnumerable<T> GetAll()
        {
            var elements = repository.GetAll();
            var retElemets = new List<T>();
            foreach (var element in elements)
            {
                retElemets.Add(mapper.MapToBll(element));
            }
            return retElemets;
        }

        public virtual T Get(int id)
        {
            return mapper.MapToBll(repository.Get(id));
        }

        public virtual T Update(T entity)
        {
            var newEntity = repository.Update(mapper.MapToDal(entity));
            uow.Commit();
            return mapper.MapToBll(newEntity);
        }
    }
}
