using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Services.Interface
{
    public interface IEventService : IService<BllEvent>
    {
        IEnumerable<BllEvent> GetEventsForUser(BllUser user);
        new BllEvent Create(BllEvent entity);
        new BllEvent Update(BllEvent entity);
    }
}
