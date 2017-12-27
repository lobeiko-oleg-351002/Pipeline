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
    public class FilepathService : Service<BllFilepath, DalFilepath, Filepath, FilepathMapper>, IFilepathService
    {

        public FilepathService(IUnitOfWork uow) : base(uow, uow.Filepaths)
        {

        }
    }
}
