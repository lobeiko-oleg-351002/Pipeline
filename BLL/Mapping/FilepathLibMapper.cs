using BLL.Mapping.Interface;
using BllEntities;
using DAL.Entities;
using DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Mapping
{
    public class FilepathLibMapper : IFilepathLibMapper
    {
        IUnitOfWork uow;

        public FilepathLibMapper() { }

        public FilepathLibMapper(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public BllFilepathLib MapToBll(DalFilepathLib entity)
        {
            BllFilepathLib bllEntity = new BllFilepathLib();
            bllEntity.Id = entity.Id;
            bllEntity.FolderName = entity.FolderName;

            IFilepathMapper mapper = new FilepathMapper();

            foreach (var item in ((IGetterByLibId<DalFilepath>)uow.Filepaths).GetEntitiesByLibId(bllEntity.Id))
            {
                BllFilepath bllSelectedEntity = mapper.MapToBll(item);
                bllEntity.Entities.Add(bllSelectedEntity);
            }
            return bllEntity;
        }

        public DalFilepathLib MapToDal(BllFilepathLib entity)
        {
            return new DalFilepathLib
            {
                Id = entity.Id,
                FolderName = entity.FolderName
            };
        }
    }
}
