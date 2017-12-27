using DAL.Entities;
using DAL.Mapping.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Mapping
{
    public class StatusMapper : IStatusMapper
    {
        public DalStatus MapToDal(Status entity)
        {
            return new DalStatus
            {
                Id = entity.id,
                Name = entity.name
            };
        }

        public Status MapToOrm(DalStatus entity)
        {
            return new Status
            {
                id = entity.Id,
                name = entity.Name
            };
        }
    }
}
