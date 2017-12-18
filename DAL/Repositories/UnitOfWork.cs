using DAL.Repositories.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        public ServiceDB context { get; }

        private GroupRepository groupRepository;
        private UserRepository userRepository;

        public IGroupRepository Groups
            => groupRepository ?? (groupRepository = new GroupRepository(context));
        public IUserRepository Users
            => userRepository ?? (userRepository = new UserRepository(context));

        public UnitOfWork(ServiceDB context)
        {
            this.context = context;
        }

        public void Commit()
        {
            context.SaveChanges();
        }
    }
}
