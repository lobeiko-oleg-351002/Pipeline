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
    public class SelectedUserMapper : IMapper<BllSelectedUser, DalSelectedUser>
    {
        IUnitOfWork uow;
        public SelectedUserMapper(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public BllSelectedUser MapToBll(DalSelectedUser entity)
        {
            IUserService EntityService = new UserService(uow);
            BllUser bllEntity = EntityService.Get(entity.Entity_id);

            BllSelectedUser bllSelectedEntity = new BllSelectedUser
            {
                Id = entity.Id,
                Entity = bllEntity,
                IsEventAccepted = entity.IsEventAccepted
            };

            return bllSelectedEntity;
        }

        public DalSelectedUser MapToDal(BllSelectedUser entity)
        {
            DalSelectedUser dalEntity = new DalSelectedUser
            {
                Id = entity.Id,
                Entity_id = entity.Entity.Id,
                IsEventAccepted = entity.IsEventAccepted
            };

            return dalEntity;
        }
    }
}
