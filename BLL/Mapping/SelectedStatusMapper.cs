using BLL.Services;
using BLL.Services.Interface;
using BllEntities;
using BllEntities.Interface;
using DAL.Entities;
using DAL.Entities.Interface;
using DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Mapping.Interface
{
    public class SelectedStatusMapper : IMapper<BllSelectedStatus, DalSelectedStatus>
    {
        IUnitOfWork uow;
        public SelectedStatusMapper(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public BllSelectedStatus MapToBll(DalSelectedStatus entity)
        {
            IStatusService EntityService = new StatusService(uow);
            BllStatus bllEntity = EntityService.Get(entity.Entity_id);

            BllSelectedStatus bllSelectedEntity = new BllSelectedStatus
            {
                Id = entity.Id,
                Entity = bllEntity,
                Date = entity.Date
            };

            return bllSelectedEntity;
        }

        public DalSelectedStatus MapToDal(BllSelectedStatus entity)
        {
            DalSelectedStatus dalEntity = new DalSelectedStatus
            {
                Id = entity.Id,
                Entity_id = entity.Entity.Id,
                Date = entity.Date
            };

            return dalEntity;
        }
    }
}
