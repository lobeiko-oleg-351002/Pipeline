using DAL.Entities;
using DAL.Mapping.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Mapping
{
    public class StatusLibMapper : IMapper<DalStatusLib, StatusLib>

    {

        public DalStatusLib MapToDal(StatusLib entity)
        {
            return new DalStatusLib
            {
                Id = entity.id,
            };
        }

        public StatusLib MapToOrm(DalStatusLib entity)
        {
            var res = new StatusLib();
            res.id = entity.Id;
            return res;
        }
    }
}
