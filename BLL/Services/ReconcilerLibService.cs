using BLL.Mapping;
using BllEntities;
using DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Services
{
    public class ReconcilerLibService
    {
        ReconcilerLibMapper libMapper;
        IUnitOfWork uow;
        public ReconcilerLibService(IUnitOfWork uow)
        {
            this.uow = uow;
            libMapper = new ReconcilerLibMapper(uow);
        }

        public BllReconcilerLib Create(BllReconcilerLib entity)
        {
            var ormLibEntity = uow.ReconcilerLibs.CreateAndReturnOrm(libMapper.MapToDal(entity));
            var lib = entity;
            uow.Commit();
            entity.Id = ormLibEntity.id;
            SelectedUserReconcilerMapper selectedEntityMapper = new SelectedUserReconcilerMapper(uow);
            foreach (var Entity in lib.SelectedEntities)
            {
                var dalEntity = selectedEntityMapper.MapToDal(Entity);
                dalEntity.Lib_id = entity.Id;
                var ormEntity = uow.SelectedUserReconcilers.Create(dalEntity);
                uow.Commit();
                Entity.Id = ormEntity.id;
            }

            return lib;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public BllReconcilerLib Get(int id)
        {
            return libMapper.MapToBll(uow.ReconcilerLibs.Get(id));
        }

        public IEnumerable<BllReconcilerLib> GetAll()
        {
            throw new NotImplementedException();
        }

        public BllReconcilerLib Update(BllReconcilerLib entity)
        {
            SelectedUserReconcilerMapper selectedEntityMapper = new SelectedUserReconcilerMapper(uow);
            foreach (var Entity in entity.SelectedEntities)
            {
                var dalEntity = selectedEntityMapper.MapToDal(Entity);
                dalEntity.Lib_id = entity.Id;
                if (Entity.Id == 0)
                {
                    var en = uow.SelectedUserReconcilers.Create(dalEntity);
                    uow.Commit();
                    Entity.Id = en.id;
                }
                else
                {
                    uow.SelectedUserReconcilers.Update(dalEntity);
                }
            }
            var EntitysWithLibId = uow.SelectedUserReconcilers.GetEntitiesByLibId(entity.Id);
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
                    uow.SelectedUserReconcilers.Delete(Entity.Id);
                }
            }
            uow.Commit();

            return entity;
        }
    }
}
