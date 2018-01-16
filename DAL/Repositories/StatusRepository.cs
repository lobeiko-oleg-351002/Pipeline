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
    public class StatusRepository : Repository<DalStatus, Status, StatusMapper>, IStatusRepository
    {
        private readonly ServiceDB context;
        public StatusRepository(ServiceDB context) : base(context)
        {
            this.context = context;
        }

        public List<DalStatus> GetAllStatusesExceptDeletedAndClosed()
        {
            var items = context.Set<Status>().Where(e => (e.name != Globals.Globals.STATUS_CLOSED) && (e.name != Globals.Globals.STATUS_DELETED));
            List<DalStatus> res = new List<DalStatus>();
            if (items.Any())
            {
                foreach (var item in items)
                {
                    res.Add(mapper.MapToDal(item));
                }
            }
            return res;
        }

        public DalStatus GetStatusClosed()
        {
            return mapper.MapToDal(context.Set<Status>().FirstOrDefault(e => e.name == Globals.Globals.STATUS_CLOSED));
        }

        public DalStatus GetStatusDeleted()
        {
            return mapper.MapToDal(context.Set<Status>().FirstOrDefault(e => e.name == Globals.Globals.STATUS_DELETED));
        }

        public bool IsContainsWithName(string name)
        {
            return context.Set<Status>().Any(e => e.name == name);
        }
    }
}
