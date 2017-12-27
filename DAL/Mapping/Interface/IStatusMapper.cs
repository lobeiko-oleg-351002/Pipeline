using DAL.Entities;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Mapping.Interface
{
    public interface IStatusMapper : IMapper<DalStatus, Status>
    {
    }
}
