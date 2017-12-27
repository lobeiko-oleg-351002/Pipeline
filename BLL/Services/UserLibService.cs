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
    public class UserLibService : EntityLibService<BllUser, UserLib, BllUserLib, SelectedUser, EntityLibMapper<BllUser, BllUserLib, UserService>, UserService>
    {
        public UserLibService(IUnitOfWork uow) : base(uow, uow.UserLibs, uow.SelectedUsers)
        {
            // this.uow = uow;
        }
    }
}
