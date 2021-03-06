﻿using DAL.Entities;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories.Interface
{
    public interface IStatusRepository : IRepository<DalStatus, Status>
    {
        bool IsContainsWithName(string name);
        List<DalStatus> GetAllStatusesExceptDeletedAndClosed();
        DalStatus GetStatusDeleted();
        DalStatus GetStatusClosed();
    }
}
