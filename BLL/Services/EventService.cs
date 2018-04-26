using BLL.Mapping;
using BLL.Services.Interface;
using BllEntities;
using DAL.Entities;
using DAL.Repositories.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Services
{
    public class EventService : Service<BllEvent, DalEvent, Event, EventMapper>, IEventService
    {

        public EventService(IUnitOfWork uow) : base(uow, uow.Events)
        {

        }

        protected override void InitMapper()
        {
            mapper = new EventMapper(uow);
        }

        public new BllEvent Create(BllEvent entity)
        {
            AttributeLibService attributeLibService = new AttributeLibService(uow);
            StatusLibService statusLibService = new StatusLibService(uow);
            UserLibService userLibService = new UserLibService(uow);
            FilepathLibService filepathLibService = new FilepathLibService(uow);
            ReconcilerLibService reconcilerLibService = new ReconcilerLibService(uow);
            entity.StatusLib = statusLibService.Create(entity.StatusLib);
            entity.RecieverLib = userLibService.Create(entity.RecieverLib);
            entity.AttributeLib = attributeLibService.Create(entity.AttributeLib);
            entity.FilepathLib = filepathLibService.Create(entity.FilepathLib);
            if (entity.ReconcilerLib != null)
            {
                entity.ReconcilerLib = reconcilerLibService.Create(entity.ReconcilerLib);
            }

            var ormEntity = uow.Events.CreateAndReturnOrm(mapper.MapToDal(entity));
            uow.Commit();

            entity.Date = ormEntity.date;
            entity.Id = ormEntity.id;

            return entity;
        }

        public new BllEvent Update(BllEvent entity)
        {
            AttributeLibService attributeLibService = new AttributeLibService(uow);
            StatusLibService statusLibService = new StatusLibService(uow);
            UserLibService userLibService = new UserLibService(uow);
            FilepathLibService filepathLibService = new FilepathLibService(uow);
            ReconcilerLibService reconcilerLibService = new ReconcilerLibService(uow);
            attributeLibService.Update(entity.AttributeLib);
            statusLibService.Update(entity.StatusLib);
            userLibService.Update(entity.RecieverLib);
            filepathLibService.Update(entity.FilepathLib);
            if (entity.ReconcilerLib != null)
            {
                reconcilerLibService.Update(entity.ReconcilerLib);
            }
            uow.Events.Update(mapper.MapToDal(entity));
            uow.Commit();

            return mapper.MapToBll(uow.Events.Get(entity.Id));

        }

        public IEnumerable<BllEvent> GetEventsForUser(BllUser user)
        {
            var events = uow.Events.GetEventsForUser(user.Id);
            List<BllEvent> result = new List<BllEvent>();
            foreach(var item in events)
            {
                result.Add(mapper.MapToBll(item));
            }
            return result;
        }
    }
}
