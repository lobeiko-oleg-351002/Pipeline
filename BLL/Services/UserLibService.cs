using BLL.Mapping;
using BllEntities;
using DAL.Repositories.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Services
{
    public class UserLibService
    {

        UserLibMapper libMapper;
        IUnitOfWork uow;
        public UserLibService(IUnitOfWork uow)
        {
            this.uow = uow;
            libMapper = new UserLibMapper(uow);
        }

        public BllUserLib Create(BllUserLib entity)
        {
            var ormLibEntity = uow.UserLibs.CreateAndReturnOrm(libMapper.MapToDal(entity));
            var lib = entity;
            uow.Commit();
            entity.Id = ormLibEntity.id;
            SelectedUserMapper selectedEntityMapper = new SelectedUserMapper(uow);
            foreach (var Entity in lib.SelectedEntities)
            {
                var dalEntity = selectedEntityMapper.MapToDal(Entity);
                dalEntity.Lib_id = entity.Id;
                var ormEntity = uow.SelectedUsers.Create(dalEntity);
                uow.Commit();
                Entity.Id = ormEntity.id;
            }

            return lib;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public BllUserLib Get(int id)
        {
            return libMapper.MapToBll(uow.UserLibs.Get(id));
        }

        public IEnumerable<BllUserLib> GetAll()
        {
            throw new NotImplementedException();
        }

        public BllUserLib Update(BllUserLib entity)
        {
            SelectedUserMapper selectedEntityMapper = new SelectedUserMapper(uow);
            foreach (var Entity in entity.SelectedEntities)
            {
                var dalEntity = selectedEntityMapper.MapToDal(Entity);
                dalEntity.Lib_id = entity.Id;
                if (Entity.Id == 0)
                {
                    var en = uow.SelectedUsers.Create(dalEntity);
                    uow.Commit();
                    Entity.Id = en.id;
                }
                else
                {
                    uow.SelectedUsers.Update(dalEntity);
                }
            }
            var EntitysWithLibId = uow.SelectedUsers.GetEntitiesByLibId(entity.Id);
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
                    uow.SelectedUsers.Delete(Entity.Id);
                }
            }
            uow.Commit();

            return entity;
        }
    }
}
