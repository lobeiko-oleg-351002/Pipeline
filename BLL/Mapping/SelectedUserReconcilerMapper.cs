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
    public class SelectedUserReconcilerMapper : IMapper<BllSelectedUserReconciler, DalSelectedUserReconciler>
    {
        IUnitOfWork uow;
        public SelectedUserReconcilerMapper(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public BllSelectedUserReconciler MapToBll(DalSelectedUserReconciler entity)
        {
            IUserService EntityService = new UserService(uow);
            BllUser bllEntity = EntityService.Get(entity.Entity_id);

            BllSelectedUserReconciler bllSelectedEntity = new BllSelectedUserReconciler
            {
                Id = entity.Id,
                Entity = bllEntity,
                IsEventReconciled = entity.IsEventReconciled,
            };

            return bllSelectedEntity;
        }

        public DalSelectedUserReconciler MapToDal(BllSelectedUserReconciler entity)
        {
            DalSelectedUserReconciler dalEntity = new DalSelectedUserReconciler
            {
                Id = entity.Id,
                Entity_id = entity.Entity.Id,
                IsEventReconciled = entity.IsEventReconciled,
            };

            return dalEntity;
        }
    }
}
