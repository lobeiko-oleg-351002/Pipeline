using BLL.Mapping.Interface;
using BllEntities;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Mapping
{
    public class StatusMapper : IStatusMapper
    {
        public StatusMapper() { }

        public DalStatus MapToDal(BllStatus entity)
        {
            DalStatus dalEntity = new DalStatus
            {
                Id = entity.Id,
                Name = entity.Name,
            };

            return dalEntity;
        }


        public BllStatus MapToBll(DalStatus entity)
        {
            BllStatus bllEntity = new BllStatus
            {
                Id = entity.Id,
                Name = entity.Name,
            };

            return bllEntity;
        }
    }
}
