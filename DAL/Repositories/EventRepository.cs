using DAL.Entities;
using DAL.Mapping;
using DAL.Repositories.Interface;
using Globals;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories
{
    public class EventRepository : Repository<DalEvent, Event, EventMapper>, IEventRepository
    {
        private readonly ServiceDB context;
        public EventRepository(ServiceDB context) : base(context)
        {
            this.context = context;
        }

        public IEnumerable<DalEvent> GetEventsForUser(int user_id)
        {
            EventMapper mapper = new EventMapper();
            var events = context.Set<Event>().Where(entity => entity.UserLib.SelectedUser.Any(e => e.User.id == user_id)
                && (entity.sender_id == user_id 
                    || (entity.approver_id != null ? entity.approver_id.Value == user_id : false)
                    || (entity.isApproved != null ? entity.isApproved.Value : false))
                 );
            var retElemets = new List<DalEvent>();
            foreach (var item in events)
            {
                retElemets.Add(mapper.MapToDal(item));
            }

            return retElemets;       
        }
    }
}
