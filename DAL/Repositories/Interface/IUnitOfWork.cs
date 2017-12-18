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

        void Commit();
    }
}
