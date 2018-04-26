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
        private SelectedEntityRepository<SelectedEventType> selectedEventTypeRepository;
        private SelectedUserRepository selectedUserRepository;
        private SelectedUserReconcilerRepository selectedUserReconcilerRepository;

        private StatusLibRepository statusLibRepository;
        private EntityLibRepository<AttributeLib> attributeLibRepository;
        private EntityLibRepository<EventTypeLib> eventTypeLibRepository;
        private UserLibRepository userLibRepository;
        private ReconcilerLibRepository reconcilerLibRepository;
        private FilepathLibRepository filepathLibRepository;

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

        public ISelectedEntityRepository<SelectedEventType> SelectedEventTypes => selectedEventTypeRepository ?? (selectedEventTypeRepository = new SelectedEntityRepository<SelectedEventType>(context));

        public ISelectedEntityRepository<SelectedAttribute> SelectedAttributes => selectedAttributeRepository ?? (selectedAttributeRepository = new SelectedEntityRepository<SelectedAttribute>(context));

        public SelectedUserRepository SelectedUsers => selectedUserRepository ?? (selectedUserRepository = new SelectedUserRepository(context));

        public SelectedUserReconcilerRepository SelectedUserReconcilers => selectedUserReconcilerRepository ?? (selectedUserReconcilerRepository = new SelectedUserReconcilerRepository(context));

        public StatusLibRepository StatusLibs => statusLibRepository ?? (statusLibRepository = new StatusLibRepository(context));

        public IRepository<IDalEntityLib, AttributeLib> AttributeLibs => attributeLibRepository ?? (attributeLibRepository = new EntityLibRepository<AttributeLib>(context));

        public IRepository<IDalEntityLib, EventTypeLib> EventTypeLibs => eventTypeLibRepository ?? (eventTypeLibRepository = new EntityLibRepository<EventTypeLib>(context));

        public UserLibRepository UserLibs => userLibRepository ?? (userLibRepository = new UserLibRepository(context));

        public ReconcilerLibRepository ReconcilerLibs => reconcilerLibRepository ?? (reconcilerLibRepository = new ReconcilerLibRepository(context));

        public FilepathLibRepository FilepathLibs => filepathLibRepository ?? (filepathLibRepository = new FilepathLibRepository(context));

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
