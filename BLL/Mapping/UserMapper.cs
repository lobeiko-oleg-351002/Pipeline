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

        public UserMapper(IUnitOfWork uow)
        {
            this.uow = uow;
            groupService = new GroupService(uow);
        }

        public UserMapper() { }

        public DalUser MapToDal(BllUser entity)
        {
            DalUser dalEntity = new DalUser
            {
                Id = entity.Id,
                Login = entity.Login,
                ChangeRights = entity.ChangeRights,
                CreateRights = entity.CreateRights,
                Fullname = entity.Fullname,
                Group_id = entity.Group != null ? entity.Group.Id : (int?)null,
                Password = entity.Password,
            };

            return dalEntity;
        }


        public BllUser MapToBll(DalUser entity)
        {
            BllUser bllEntity = new BllUser
            {
                Id = entity.Id,
                Login = entity.Login,
                CreateRights = entity.CreateRights,
                ChangeRights = entity.ChangeRights,
                Fullname = entity.Fullname,
                Password = entity.Password,
                Group = entity.Group_id != null ? groupService.Get(entity.Group_id.Value) : null
            };

            return bllEntity;
        }
    }
}
