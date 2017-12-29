using BLL.Mapping;
using BLL.Services.Interface;
using BllEntities;
using DAL.Entities;
using DAL.Repositories.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Services
{
    public class FilepathLibService
    {
        IUnitOfWork uow;
        FilepathLibMapper mapper;
        public FilepathLibService(IUnitOfWork uow) 
        {
             this.uow = uow;
            mapper = new FilepathLibMapper(uow);
        }

        public BllFilepathLib Create(BllFilepathLib entity)
        {
            var ormLib = uow.FilepathLibs.Create(mapper.MapToDal(entity));
            uow.Commit();
            entity.Id = ormLib.id;
            FilepathMapper EntityMapper = new FilepathMapper(uow);
            foreach (var Entity in entity.Entities)
            {
                var dalEntity = EntityMapper.MapToDal(Entity);
                dalEntity.Lib_id = entity.Id;
                var ormEntity = uow.Filepaths.Create(dalEntity);
                uow.Commit();
                Entity.Id = ormEntity.id;
            }
            return entity;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public BllFilepathLib Get(int id)
        {
            var retElement = mapper.MapToBll(uow.FilepathLibs.Get(id));
            return retElement;
        }

        public IEnumerable<BllFilepathLib> GetAll()
        {
            throw new NotImplementedException();
        }

        public BllFilepathLib Update(BllFilepathLib entity)
        {
            FilepathMapper EntityMapper = new FilepathMapper();
            foreach (var Entity in entity.Entities)
            {
                var dalEntity = EntityMapper.MapToDal(Entity);
                dalEntity.Lib_id = entity.Id;
                if (Entity.Id == 0)
                {
                    var ormEntity = uow.Filepaths.Create(dalEntity);
                    uow.Commit();
                    Entity.Id = ormEntity.id;
                }
                else
                {
                    uow.Filepaths.Update(dalEntity);
                }
            }

            var EntitysWithLibId = ((IGetterByLibId<DalFilepath>)uow.Filepaths).GetEntitiesByLibId(entity.Id);
            foreach (var Entity in EntitysWithLibId)
            {
                bool isTrashEntity = true;
                foreach (var item in entity.Entities)
                {
                    if (item.Id == Entity.Id)
                    {
                        isTrashEntity = false;
                        break;
                    }
                }
                if (isTrashEntity == true)
                {
                    uow.Filepaths.Delete(Entity.Id);
                }
            }
            uow.Commit();

            return entity;
        }
    }
}
