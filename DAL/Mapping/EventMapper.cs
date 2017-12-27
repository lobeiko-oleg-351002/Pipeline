using DAL.Entities;
using DAL.Mapping.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Mapping
{
    public class EventMapper : IEventMapper
    {
        public DalEvent MapToDal(Event entity)
        {
            return new DalEvent
            {
                Id = entity.id,
                Attribute_lib_id = entity.attribute_lib_id,
                Date = entity.date,
                Description = entity.description,
                Filepath_lib_id = entity.filepath_lib_id,
                Sender_id = entity.sender_id,
                Type_id = entity.type_id,
                Name = entity.name,
                Receiver_lib_id = entity.receiver_lib_id,
                Status_lib_id = entity.status_lib_id
            };
        }

        public Event MapToOrm(DalEvent entity)
        {
            return new Event
            {
                id = entity.Id,
                attribute_lib_id = entity.Attribute_lib_id,
                date = entity.Date,
                description = entity.Description,
                filepath_lib_id = entity.Filepath_lib_id,
                sender_id = entity.Sender_id,
                type_id = entity.Type_id,
                name = entity.Name,
                receiver_lib_id = entity.Receiver_lib_id,
                status_lib_id = entity.Status_lib_id
            };
        }
    }
}
