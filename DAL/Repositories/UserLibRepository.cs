using DAL.Entities.Interface;
using DAL.Mapping;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories
{
    public class UserLibRepository : Repository<DalUserLib, UserLib, UserLibMapper>
    {
        private readonly ServiceDB context;

        public UserLibRepository(ServiceDB context) : base(context)
        {
            this.context = context;
        }
    }
}
