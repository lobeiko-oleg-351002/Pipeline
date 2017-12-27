using DAL.Entities;
using DAL.Mapping;
using DAL.Repositories.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories
{
    public class StatusRepository : Repository<DalStatus, Status, StatusMapper>, IStatusRepository
    {
        private readonly ServiceDB context;
        public StatusRepository(ServiceDB context) : base(context)
        {
            this.context = context;
        }
    }
}
