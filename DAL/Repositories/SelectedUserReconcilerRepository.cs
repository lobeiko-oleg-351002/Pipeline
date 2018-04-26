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
    public class SelectedUserReconcilerRepository : ISelectedEntityRepository<SelectedUserReconciler>
    {
        private readonly ServiceDB context;
        SelectedUserReconcilerMapper mapper;
        public SelectedUserReconcilerRepository(ServiceDB context)
        {
            this.context = context;
            mapper = new SelectedUserReconcilerMapper();
        }

        public SelectedUserReconciler Create(IDalSelectedEntity entity)
        {
            var res = context.Set<SelectedUserReconciler>().Add(mapper.MapToOrm((DalSelectedUserReconciler)entity));
            return res;
        }

        public SelectedUserReconciler CreateAndReturnOrm(IDalSelectedEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            var ormEntity = context.Set<SelectedUserReconciler>().Single(e => e.id == id);
            context.Set<SelectedUserReconciler>().Remove(ormEntity);
        }

        public IDalSelectedEntity Get(int id)
        {
            var ormEntity = context.Set<SelectedUserReconciler>().FirstOrDefault(e => e.id == id);
            return ormEntity != null ? (mapper.MapToDal(ormEntity)) : null;
        }

        public IEnumerable<IDalSelectedEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDalSelectedEntity> GetEntitiesByLibId(int id)
        {
            var elements = context.Set<SelectedUserReconciler>().Where(User => User.lib_id == id);
            var retElemets = new List<DalSelectedUserReconciler>();
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
            var ormEntity = context.Set<SelectedUserReconciler>().Find(entity.Id);
            if (ormEntity != null)
            {
                context.Entry(ormEntity).CurrentValues.SetValues(mapper.MapToOrm((DalSelectedUserReconciler)entity));
            }
        }

        IDalSelectedEntity IRepository<IDalSelectedEntity, SelectedUserReconciler>.Create(IDalSelectedEntity entity)
        {
            throw new NotImplementedException();
        }

        IDalSelectedEntity IRepository<IDalSelectedEntity, SelectedUserReconciler>.Update(IDalSelectedEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
