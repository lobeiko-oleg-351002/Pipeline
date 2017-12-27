using DAL.Entities;
using DAL.Mapping.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Mapping
{
    public class FilepathMapper : IFilepathMapper
    {
        public DalFilepath MapToDal(Filepath entity)
        {
            return new DalFilepath
            {
                Id = entity.id,
                Path = entity.path,
                Lib_id = entity.lib_id
            };
        }

        public Filepath MapToOrm(DalFilepath entity)
        {
            return new Filepath
            {
                id = entity.Id,
                path = entity.Path,
                lib_id = entity.Lib_id
            };
        }
    }
}
