using DAL.Entities;
using DAL.Mapping.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Mapping
{
    public class SelectedStatusMapper : IMapper<DalSelectedStatus, SelectedStatus>
    {
        public DalSelectedStatus MapToDal(SelectedStatus entity)
        {
            return new DalSelectedStatus
            {
                Id = entity.id,
                Lib_id = entity.lib_id,
                Entity_id = entity.entity_id,
                Date = entity.date
            };
        }

        public SelectedStatus MapToOrm(DalSelectedStatus entity)
        {
            return new SelectedStatus
            {
                id = entity.Id,
                entity_id = entity.Entity_id,
                lib_id = entity.Lib_id,
                 date = entity.Date
            };
        }
    }
}
