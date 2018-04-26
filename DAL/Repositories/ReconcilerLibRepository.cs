using DAL.Entities;
using DAL.Mapping;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories
{
    public class ReconcilerLibRepository : Repository<DalReconcilerLib, ReconcilerLib, ReconcilerLibMapper>
    {
        private readonly ServiceDB context;

        public ReconcilerLibRepository(ServiceDB context) : base(context)
        {
            this.context = context;
        }
    }
}
