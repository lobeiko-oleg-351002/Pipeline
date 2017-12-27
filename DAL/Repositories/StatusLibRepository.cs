using DAL.Entities;
using DAL.Mapping;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories
{
    public class StatusLibRepository : Repository<DalStatusLib, StatusLib, StatusLibMapper>
    {
        private readonly ServiceDB context;

        public StatusLibRepository(ServiceDB context) : base(context)
        {
            this.context = context;
        }
    }
}
