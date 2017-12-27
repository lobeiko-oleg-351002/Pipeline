using DAL.Entities;
using DAL.Mapping;
using DAL.Repositories.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories
{
    public class EventTypeRepository : Repository<DalEventType, EventType, EventTypeMapper>, IEventTypeRepository
    {
        private readonly ServiceDB context;
        public EventTypeRepository(ServiceDB context) : base(context)
        {
            this.context = context;
        }
    }
}
