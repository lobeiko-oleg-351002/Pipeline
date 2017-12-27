using DAL.Entities.Interface;
using DAL.Mapping;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories
{
    public class EntityLibRepository<UEntity> : Repository<IDalEntityLib, UEntity, EntityLibMapper<UEntity>>
        where UEntity : class, IOrmEntity
    {
        private readonly ServiceDB context;

        public EntityLibRepository(ServiceDB context) : base(context)
        {
            this.context = context;
        }
    }
}
