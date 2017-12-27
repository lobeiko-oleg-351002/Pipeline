using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Services.Interface
{
    public interface IUserService : IService<BllUser>
    {
        BllUser GetUserByLogin(string login);
        BllUser Authorize(string login, string password);
        IEnumerable<BllUser> GetUsersByGroup(int group_id);
        new BllUser Create(BllUser entity);
    }
}
