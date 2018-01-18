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
    public class SelectedStatusRepository :  ISelectedEntityRepository<SelectedStatus>
    {
        private readonly ServiceDB context;
        SelectedStatusMapper mapper;
        public SelectedStatusRepository(ServiceDB context) 
        {
            this.context = context;
            mapper = new SelectedStatusMapper();
        }

        public SelectedStatus Create(IDalSelectedEntity entity)
        {
            var res = context.Set<SelectedStatus>().Add(mapper.MapToOrm((DalSelectedStatus)entity));
            return res;
        }

        public SelectedStatus CreateAndReturnOrm(IDalSelectedEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            var ormEntity = context.Set<SelectedStatus>().Single(e => e.id == id);
            context.Set<SelectedStatus>().Remove(ormEntity);
        }

        public IDalSelectedEntity Get(int id)
        {
            var ormEntity = context.Set<SelectedStatus>().FirstOrDefault(e => e.id == id);
            return ormEntity != null ? (mapper.MapToDal(ormEntity)) : null;
        }

        public IEnumerable<IDalSelectedEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDalSelectedEntity> GetEntitiesByLibId(int id)
        {
            var elements = context.Set<SelectedStatus>().Where(Status => Status.lib_id == id);
            var retElemets = new List<DalSelectedStatus>();
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
            throw new NotImplementedException();
        }

        IDalSelectedEntity IRepository<IDalSelectedEntity, SelectedStatus>.Create(IDalSelectedEntity entity)
        {
            throw new NotImplementedException();
        }

        IDalSelectedEntity IRepository<IDalSelectedEntity, SelectedStatus>.Update(IDalSelectedEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
