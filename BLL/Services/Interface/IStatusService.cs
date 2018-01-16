using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Services.Interface
{
    public interface IStatusService : IService<BllStatus>
    {
        bool IsContainsWithName(string name);
        List<BllStatus> GetAllStatusesExceptDeletedAndClosed();
        BllStatus GetStatusDeleted();
        BllStatus GetStatusClosed();
    }
}
