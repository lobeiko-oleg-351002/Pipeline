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
    public class StatusService : Service<BllStatus, DalStatus, Status, StatusMapper>, IStatusService
    {

        public StatusService(IUnitOfWork uow) : base(uow, uow.Statuses)
        {

        }

        public List<BllStatus> GetAllStatusesExceptDeletedAndClosed()
        { 
            var items = uow.Statuses.GetAllStatusesExceptDeletedAndClosed();
            List<BllStatus> res = new List<BllStatus>();
            if (items.Any())
            {
                foreach (var item in items)
                {
                    res.Add(mapper.MapToBll(item));
                }
            }
            return res;
        }

        public BllStatus GetStatusClosed()
        {
            return mapper.MapToBll(uow.Statuses.GetStatusClosed());
        }

        public BllStatus GetStatusDeleted()
        {
            return mapper.MapToBll(uow.Statuses.GetStatusDeleted());
        }

        public bool IsContainsWithName(string name)
        {
            return uow.Statuses.IsContainsWithName(name);
        }
    }
}
