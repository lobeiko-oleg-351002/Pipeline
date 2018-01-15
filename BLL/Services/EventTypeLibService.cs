using BLL.Mapping;
using BllEntities;
using DAL.Repositories.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Services
{
    public class EventTypeLibService : EntityLibService<BllEventType, EventTypeLib, BllEventTypeLib, SelectedEventType, EntityLibMapper<BllEventType, BllEventTypeLib, EventTypeService>, EventTypeService>
    {
        public EventTypeLibService(IUnitOfWork uow) : base(uow, uow.EventTypeLibs, uow.SelectedEventTypes)
        {
            // this.uow = uow;
        }
    }
}
