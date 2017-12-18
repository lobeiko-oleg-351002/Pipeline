using BLL.Mapping.Interface;
using BllEntities;
using DAL.Entities;
using DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Mapping
{
    public class GroupMapper : IGroupMapper
    {
        IUnitOfWork uow;

        public GroupMapper(IUnitOfWork uow)
        {
            this.uow = uow;

        }

        public GroupMapper() { }

        public DalGroup MapToDal(BllGroup entity)
        {
            DalGroup dalEntity = new DalGroup
            {
                Id = entity.Id,
                Name = entity.Name,
            };

            return dalEntity;
        }


        public BllGroup MapToBll(DalGroup entity)
        {
            BllGroup bllEntity = new BllGroup
            {
                Id = entity.Id,
                Name = entity.Name,
            };

            return bllEntity;
        }
    }
}
