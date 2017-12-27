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
    public class FilepathMapper : IFilepathMapper
    {
        public FilepathMapper() { }

        public FilepathMapper(IUnitOfWork uow) { }

        public DalFilepath MapToDal(BllFilepath entity)
        {
            DalFilepath dalEntity = new DalFilepath
            {
                Id = entity.Id,
                Path = entity.Path
            };

            return dalEntity;
        }


        public BllFilepath MapToBll(DalFilepath entity)
        {
            BllFilepath bllEntity = new BllFilepath
            {
                Id = entity.Id,
                Path = entity.Path,
            };

            return bllEntity;
        }
    }
}
