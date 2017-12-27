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
    public class FilepathLibService : EntitySimpleLibService<BllFilepath, BllFilepathLib, DalFilepath, FilepathMapper, FilepathLib, Filepath>, IFilepathLibService
    {
        public FilepathLibService(IUnitOfWork uow) : base(uow, uow.FilepathLibs, uow.Filepaths)
        {
            // this.uow = uow;
        }
    }
}
