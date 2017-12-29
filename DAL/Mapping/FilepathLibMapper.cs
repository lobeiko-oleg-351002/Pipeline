using DAL.Entities;
using DAL.Mapping.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Mapping
{
    public class FilepathLibMapper : IFilepathLibMapper
    {
        public DalFilepathLib MapToDal(FilepathLib entity)
        {
            return new DalFilepathLib
            {
                Id = entity.id,
                FolderName = entity.folderName
            };
        }

        public FilepathLib MapToOrm(DalFilepathLib entity)
        {
            return new FilepathLib
            {
                id = entity.Id,
                folderName = entity.FolderName
            };
        }
    }
}
