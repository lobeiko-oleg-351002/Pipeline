using BLL.Mapping;
using BLL.Services.Interface;
using BllEntities;
using DAL.Entities;
using DAL.Repositories.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Services
{
    public class GroupService : Service<BllGroup, DalGroup, Group, GroupMapper>, IGroupService
    {
        // private readonly IUnitOfWork uow;

        public GroupService(IUnitOfWork uow) : base(uow, uow.Groups)
        {

        }
    }
}
