using BLL.Mapping.Interface;
using BLL.Services;
using BllEntities;
using DAL.Entities;
using DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Mapping
{
    public class UserMapper : IUserMapper
    {
        IUnitOfWork uow;
        GroupService groupService;
        StatusLibService statusLibService;
        EventTypeLibService eventTypeLibService;

        public UserMapper(IUnitOfWork uow)
        {
            this.uow = uow;
            groupService = new GroupService(uow);
            statusLibService = new StatusLibService(uow);
            eventTypeLibService = new EventTypeLibService(uow);
        }

        public UserMapper() { }

        public DalUser MapToDal(BllUser entity)
        {
            DalUser dalEntity = new DalUser
            {
                Id = entity.Id,
                Login = entity.Login,
                Fullname = entity.Fullname,
                Group_id = entity.Group.Id,
                Password = entity.Password,
                EventTypeLib_id = entity.EventTypeLib.Id,
                StatusLib_id = entity.StatusLib.Id
            };

            return dalEntity;
        }


        public BllUser MapToBll(DalUser entity)
        {
            BllUser bllEntity = new BllUser
            {
                Id = entity.Id,
                Login = entity.Login,
                Fullname = entity.Fullname,
                Password = entity.Password,
                Group =  groupService.Get(entity.Group_id),
                EventTypeLib = eventTypeLibService.Get(entity.EventTypeLib_id),
                StatusLib = statusLibService.Get(entity.StatusLib_id)
            };

            return bllEntity;
        }
    }
}
