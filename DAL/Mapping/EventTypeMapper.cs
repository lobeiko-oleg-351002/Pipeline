using DAL.Entities;
using DAL.Mapping.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Mapping
{
    public class EventTypeMapper : IEventTypeMapper
    {
        public DalEventType MapToDal(EventType entity)
        {
            return new DalEventType
            {
                Id = entity.id,
                Name = entity.name
            };
        }

        public EventType MapToOrm(DalEventType entity)
        {
            return new EventType
            {
                id = entity.Id,
                name = entity.Name,
            };
        }
    }
}
