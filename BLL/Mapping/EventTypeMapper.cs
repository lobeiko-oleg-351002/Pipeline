using BLL.Mapping.Interface;
using BllEntities;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Mapping
{
    public class EventTypeMapper : IEventTypeMapper
    {
        public EventTypeMapper() { }

        public DalEventType MapToDal(BllEventType entity)
        {
            DalEventType dalEntity = new DalEventType
            {
                Id = entity.Id,
                Name = entity.Name,
            };

            return dalEntity;
        }


        public BllEventType MapToBll(DalEventType entity)
        {
            BllEventType bllEntity = new BllEventType
            {
                Id = entity.Id,
                Name = entity.Name,
            };

            return bllEntity;
        }
    }
}
