using DAL.Entities;
using DAL.Mapping.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Mapping
{
    public class GroupMapper : IGroupMapper
    {
        public DalGroup MapToDal(Group entity)
        {
            return new DalGroup
            {
                Id = entity.id,
                Name = entity.name
            };
        }

        public Group MapToOrm(DalGroup entity)
        {
            return new Group
            {
                id = entity.Id,
                name = entity.Name
            };
        }
    }
}
