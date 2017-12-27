using DAL.Entities.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories.Interface
{
    public interface IUnitOfWork
    {
        IGroupRepository Groups { get; }
        IUserRepository Users { get; }
        IAttributeRepository Attributes { get; }
        IStatusRepository Statuses { get; }
        IEventRepository Events { get; }
        IEventTypeRepository EventTypes { get; }
        IFilepathRepository Filepaths { get; }

        SelectedStatusRepository SelectedStatuses { get; }
        ISelectedEntityRepository<SelectedAttribute> SelectedAttributes { get; }
        ISelectedEntityRepository<SelectedUser> SelectedUsers { get; }

        StatusLibRepository StatusLibs { get; }
        IRepository<IDalEntityLib, AttributeLib> AttributeLibs { get; }
        IRepository<IDalEntityLib, UserLib> UserLibs { get; }
        IRepository<IDalEntityLib, FilepathLib> FilepathLibs { get; }

        void Commit();
    }
}
