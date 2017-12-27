using BLL.Mapping.Interface;
using BLL.Services;
using BLL.Services.Interface;
using BllEntities;
using DAL.Entities;
using DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Mapping
{
    public class EventMapper : IEventMapper
    {
        IUnitOfWork uow;
        AttributeLibService attributeLibService;
        StatusLibService statusLibService;
        UserLibService userLibService;
        FilepathLibService filepathLibService;
        UserService userService;
        EventTypeService eventTypeService;

        public EventMapper(IUnitOfWork uow)
        {
            this.uow = uow;
            attributeLibService = new AttributeLibService(uow);
            statusLibService = new StatusLibService(uow);
            userLibService = new UserLibService(uow);
            filepathLibService = new FilepathLibService(uow);
            userService = new UserService(uow);
            eventTypeService = new EventTypeService(uow);
        }

        public EventMapper() { }

        public DalEvent MapToDal(BllEvent entity)
        {
            DalEvent dalEntity = new DalEvent
            {
                Id = entity.Id,
                Name = entity.Name,
                Attribute_lib_id = entity.AttributeLib.Id,
                Date = entity.Date,
                Description = entity.Description,
                Filepath_lib_id = entity.FilepathLib.Id,
                Receiver_lib_id = entity.RecieverLib.Id,
                Status_lib_id = entity.StatusLib.Id,
                Sender_id = entity.Sender.Id,
                Type_id = entity.Type.Id
            };

            return dalEntity;
        }


        public BllEvent MapToBll(DalEvent entity)
        {
            BllEvent bllEntity = new BllEvent
            {
                Id = entity.Id,
                Name = entity.Name,
                AttributeLib = attributeLibService.Get(entity.Attribute_lib_id),
                Date = entity.Date,
                Description = entity.Description,
                FilepathLib = filepathLibService.Get(entity.Filepath_lib_id),
                RecieverLib = userLibService.Get(entity.Receiver_lib_id),
                StatusLib = statusLibService.Get(entity.Status_lib_id),
                Sender = userService.Get(entity.Sender_id),
                Type = eventTypeService.Get(entity.Type_id)
            };

            return bllEntity;
        }
    }
}
