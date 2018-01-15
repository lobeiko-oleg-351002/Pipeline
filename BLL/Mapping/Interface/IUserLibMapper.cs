using BllEntities;
using DAL.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Mapping.Interface
{
    public interface IUserLibMapper : IMapper<BllUserLib, DalUserLib>
    {
    }
}
