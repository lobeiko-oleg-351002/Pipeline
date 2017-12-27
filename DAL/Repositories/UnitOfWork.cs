using DAL.Entities.Interface;
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
        private AttributeRepository attributeRepository;
        private StatusRepository statusRepository;
        private EventRepository eventRepository;
        private EventTypeRepository eventTypeRepository;
        private FilepathRepository filepathRepository;

        private SelectedStatusRepository selectedStatusRepository;
        private SelectedEntityRepository<SelectedAttribute> selectedAttributeRepository;
        private SelectedEntityRepository<SelectedUser> selectedUserRepository;

        private StatusLibRepository statusLibRepository;
        private EntityLibRepository<AttributeLib> attributeLibRepository;
        private EntityLibRepository<UserLib> userLibRepository;
        private EntityLibRepository<FilepathLib> filepathLibRepository;

        public IGroupRepository Groups
            => groupRepository ?? (groupRepository = new GroupRepository(context));
        public IUserRepository Users
            => userRepository ?? (userRepository = new UserRepository(context));

        public IAttributeRepository Attributes => attributeRepository ?? (attributeRepository = new AttributeRepository(context));

        public IStatusRepository Statuses => statusRepository ?? (statusRepository = new StatusRepository(context));

        public IEventRepository Events => eventRepository ?? (eventRepository = new EventRepository(context));

        public IEventTypeRepository EventTypes => eventTypeRepository ?? (eventTypeRepository = new EventTypeRepository(context));

        public IFilepathRepository Filepaths => filepathRepository ?? (filepathRepository = new FilepathRepository(context));

        public SelectedStatusRepository SelectedStatuses => selectedStatusRepository ?? (selectedStatusRepository = new SelectedStatusRepository(context));

        public ISelectedEntityRepository<SelectedAttribute> SelectedAttributes => selectedAttributeRepository ?? (selectedAttributeRepository = new SelectedEntityRepository<SelectedAttribute>(context));

        public ISelectedEntityRepository<SelectedUser> SelectedUsers => selectedUserRepository ?? (selectedUserRepository = new SelectedEntityRepository<SelectedUser>(context));

        public StatusLibRepository StatusLibs => statusLibRepository ?? (statusLibRepository = new StatusLibRepository(context));

        public IRepository<IDalEntityLib, AttributeLib> AttributeLibs => attributeLibRepository ?? (attributeLibRepository = new EntityLibRepository<AttributeLib>(context));

        public IRepository<IDalEntityLib, UserLib> UserLibs => userLibRepository ?? (userLibRepository = new EntityLibRepository<UserLib>(context));

        public IRepository<IDalEntityLib, FilepathLib> FilepathLibs => filepathLibRepository ?? (filepathLibRepository = new EntityLibRepository<FilepathLib>(context));

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
