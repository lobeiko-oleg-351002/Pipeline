using BLL.Mapping;
using BLL.Mapping.Interface;
using BLL.Services.Interface;
using BllEntities;
using DAL.Entities;
using DAL.Repositories;
using DAL.Repositories.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Services
{
    public class StatusLibService 
    {

        StatusLibMapper libMapper;
        IUnitOfWork uow;
        public StatusLibService(IUnitOfWork uow) 
        {
            this.uow = uow;
            libMapper = new StatusLibMapper(uow);
        }

        public BllStatusLib Create(BllStatusLib entity)
        {
            var ormLibEntity = uow.StatusLibs.Create(libMapper.MapToDal(entity));
            var lib = entity;
            uow.Commit();
            entity.Id = ormLibEntity.id;
            SelectedStatusMapper selectedEntityMapper = new SelectedStatusMapper(uow);
            foreach (var Entity in lib.SelectedEntities)
            {
                var dalEntity = selectedEntityMapper.MapToDal(Entity);
                dalEntity.Lib_id = entity.Id;
                var ormEntity = uow.SelectedStatuses.Create(dalEntity);
                uow.Commit();
                Entity.Id = ormEntity.id;
            }

            return lib;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public BllStatusLib Get(int id)
        {
            return libMapper.MapToBll((DalStatusLib)uow.StatusLibs.Get(id));
        }

        public IEnumerable<BllStatusLib> GetAll()
        {
            throw new NotImplementedException();
        }

        public BllStatusLib Update(BllStatusLib entity)
        {
            SelectedStatusMapper selectedEntityMapper = new SelectedStatusMapper(uow);
            foreach (var Entity in entity.SelectedEntities)
            {
                var dalEntity = selectedEntityMapper.MapToDal(Entity);
                dalEntity.Lib_id = entity.Id;
                if (Entity.Id == 0)
                {
                   var en =  uow.SelectedStatuses.Create(dalEntity);
                    uow.Commit();
                    Entity.Id = en.id;
                }
                //else
                //{
                //    uow.SelectedStatuses.Update(dalEntity);
                //}
            }
            var EntitysWithLibId = uow.SelectedStatuses.GetEntitiesByLibId(entity.Id);
            foreach (var Entity in EntitysWithLibId)
            {
                bool isTrashEntity = true;
                foreach (var selectedEntity in entity.SelectedEntities)
                {
                    if (Entity.Id == selectedEntity.Id)
                    {
                        isTrashEntity = false;
                        break;
                    }
                }
                if (isTrashEntity == true)
                {
                    uow.SelectedStatuses.Delete(Entity.Id);
                }
            }
            uow.Commit();

            return entity;
        }
    }
}
