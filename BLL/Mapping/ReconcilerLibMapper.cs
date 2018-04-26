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
    public class ReconcilerLibMapper : IReconcilerLibMapper
    {
        private readonly IUnitOfWork uow;
        private readonly SelectedUserReconcilerRepository repository;

        public ReconcilerLibMapper() { }

        public ReconcilerLibMapper(IUnitOfWork uow)
        {
            this.uow = uow;
            repository = uow.SelectedUserReconcilers;
        }

        public DalReconcilerLib MapToDal(BllReconcilerLib entity)
        {
            DalReconcilerLib dalEntity = new DalReconcilerLib
            {
                Id = entity.Id,
            };

            return dalEntity;
        }


        public BllReconcilerLib MapToBll(DalReconcilerLib entity)
        {
            BllReconcilerLib bllEntity = new BllReconcilerLib();
            bllEntity.Id = entity.Id;

            SelectedUserReconcilerMapper selectedEntityMapper = new SelectedUserReconcilerMapper(uow);

            foreach (var item in repository.GetEntitiesByLibId(bllEntity.Id))
            {
                BllSelectedUserReconciler bllSelectedEntity = selectedEntityMapper.MapToBll((DalSelectedUserReconciler)item);
                bllEntity.SelectedEntities.Add(bllSelectedEntity);
            }
            return bllEntity;
        }
    }
}
