using DAL.Entities;
using DAL.Mapping;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories
{
    public class FilepathLibRepository : Repository<DalFilepathLib, FilepathLib, FilepathLibMapper>
    {
        private readonly ServiceDB context;

        public FilepathLibRepository(ServiceDB context) : base(context)
        {
            this.context = context;
        }
    }
}
