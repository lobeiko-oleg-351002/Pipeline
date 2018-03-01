using DAL.Entities;
using DAL.Entities.Interface;
using DAL.Mapping;
using DAL.Repositories.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories
{
    public class SelectedUserRepository : ISelectedEntityRepository<SelectedUser>
    {
        private readonly ServiceDB context;
        SelectedUserMapper mapper;
        public SelectedUserRepository(ServiceDB context)
        {
            this.context = context;
            mapper = new SelectedUserMapper();
        }

        public SelectedUser Create(IDalSelectedEntity entity)
        {
            var res = context.Set<SelectedUser>().Add(mapper.MapToOrm((DalSelectedUser)entity));
            return res;
        }

        public SelectedUser CreateAndReturnOrm(IDalSelectedEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            var ormEntity = context.Set<SelectedUser>().Single(e => e.id == id);
            context.Set<SelectedUser>().Remove(ormEntity);
        }

        public IDalSelectedEntity Get(int id)
        {
            var ormEntity = context.Set<SelectedUser>().FirstOrDefault(e => e.id == id);
            return ormEntity != null ? (mapper.MapToDal(ormEntity)) : null;
        }

        public IEnumerable<IDalSelectedEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDalSelectedEntity> GetEntitiesByLibId(int id)
        {
            var elements = context.Set<SelectedUser>().Where(User => User.lib_id == id);
            var retElemets = new List<DalSelectedUser>();
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    retElemets.Add(mapper.MapToDal(element));
                }
            }

            return retElemets;
        }

        public void Update(IDalSelectedEntity entity)
        {
            var ormEntity = context.Set<SelectedUser>().Find(entity.Id);
            if (ormEntity != null)
            {
                context.Entry(ormEntity).CurrentValues.SetValues(mapper.MapToOrm((DalSelectedUser)entity));
            }
        }

        IDalSelectedEntity IRepository<IDalSelectedEntity, SelectedUser>.Create(IDalSelectedEntity entity)
        {
            throw new NotImplementedException();
        }

        IDalSelectedEntity IRepository<IDalSelectedEntity, SelectedUser>.Update(IDalSelectedEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
