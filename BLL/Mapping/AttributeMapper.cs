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
    public class AttributeMapper : IAttributeMapper
    {

        public AttributeMapper() { }

        public DalAttribute MapToDal(BllAttribute entity)
        {
            DalAttribute dalEntity = new DalAttribute
            {
                Id = entity.Id,
                Name = entity.Name,
            };

            return dalEntity;
        }


        public BllAttribute MapToBll(DalAttribute entity)
        {
            BllAttribute bllEntity = new BllAttribute
            {
                Id = entity.Id,
                Name = entity.Name,
            };

            return bllEntity;
        }
    }
}
