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
    public class GroupRepository : Repository<DalGroup, Group, GroupMapper>, IGroupRepository
    {
        private readonly ServiceDB context;
        public GroupRepository(ServiceDB context) : base(context)
        {
            this.context = context;
        }
    }
}
