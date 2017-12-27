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
            var user = context.Set<User>().FirstOrDefault(entity => entity.id == user_id);
            
            var retElemets = new List<DalEvent>();
            foreach (var element in user.SelectedUser)
            {
                var ev = element.UserLib.Event;
                if (ev.Count != 0)
                {
                    retElemets.Add(mapper.MapToDal(ev.First()));
                }
            }
            return retElemets;       
        }
    }
}
