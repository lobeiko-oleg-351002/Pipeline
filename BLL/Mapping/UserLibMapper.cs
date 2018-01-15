using BLL.Mapping.Interface;
using BllEntities;
using DAL.Entities;
using DAL.Entities.Interface;
using DAL.Repositories;
using DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Mapping
{
    public class UserLibMapper : IUserLibMapper
    {
        private readonly IUnitOfWork uow;
        private readonly SelectedUserRepository repository;

        public UserLibMapper() { }

        public UserLibMapper(IUnitOfWork uow)
        {
            this.uow = uow;
            repository = uow.SelectedUsers;
        }

        public DalUserLib MapToDal(BllUserLib entity)
        {
            DalUserLib dalEntity = new DalUserLib
            {
                Id = entity.Id,
            };

            return dalEntity;
        }


        public BllUserLib MapToBll(DalUserLib entity)
        {
            BllUserLib bllEntity = new BllUserLib();
            bllEntity.Id = entity.Id;

            SelectedUserMapper selectedEntityMapper = new SelectedUserMapper(uow);

            foreach (var item in repository.GetEntitiesByLibId(bllEntity.Id))
            {
                BllSelectedUser bllSelectedEntity = selectedEntityMapper.MapToBll((DalSelectedUser)item);
                bllEntity.SelectedEntities.Add(bllSelectedEntity);
            }
            return bllEntity;
        }
    }
}
