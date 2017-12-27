using BLL.Mapping.Interface;
using BllEntities;
using DAL.Entities;
using DAL.Repositories;
using DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Mapping
{
    public class StatusLibMapper : IStatusLibMapper
    {
        private readonly IUnitOfWork uow;
        private readonly SelectedStatusRepository repository;

        public StatusLibMapper() { }

        public StatusLibMapper(IUnitOfWork uow)
        {
            this.uow = uow;
            repository = uow.SelectedStatuses;
        }

        public DalStatusLib MapToDal(BllStatusLib entity)
        {
            DalStatusLib dalEntity = new DalStatusLib
            {
                Id = entity.Id,
            };

            return dalEntity;
        }


        public BllStatusLib MapToBll(DalStatusLib entity)
        {
            BllStatusLib bllEntity = new BllStatusLib();
            bllEntity.Id = entity.Id;

            SelectedStatusMapper selectedEntityMapper = new SelectedStatusMapper(uow);

            foreach (var item in repository.GetEntitiesByLibId(bllEntity.Id))
            {
                BllSelectedStatus bllSelectedEntity = selectedEntityMapper.MapToBll((DalSelectedStatus)item);
                bllEntity.SelectedEntities.Add(bllSelectedEntity);
            }
            return bllEntity;
        }
    }
}
