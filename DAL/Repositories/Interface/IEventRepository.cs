using DAL.Entities;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories.Interface
{
    public interface IEventRepository : IRepository<DalEvent, Event>
    {
        IEnumerable<DalEvent> GetEventsForUser(int user_id);
    }
}
