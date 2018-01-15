using DAL.Entities;
using DAL.Mapping.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Mapping
{
    public class SelectedUserMapper : IMapper<DalSelectedUser, SelectedUser>
    {
        public DalSelectedUser MapToDal(SelectedUser entity)
        {
            return new DalSelectedUser
            {
                Id = entity.id,
                Lib_id = entity.lib_id,
                Entity_id = entity.entity_id,
                IsEventAccepted = entity.isEventAccepted
            };
        }

        public SelectedUser MapToOrm(DalSelectedUser entity)
        {
            return new SelectedUser
            {
                id = entity.Id,
                entity_id = entity.Entity_id,
                lib_id = entity.Lib_id,
                isEventAccepted = entity.IsEventAccepted
            };
        }
    }
}
