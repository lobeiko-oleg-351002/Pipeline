using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Entities.Interface
{
    public interface IDalEntityWithLibId : IDalEntity
    {
        int Lib_id { get; set; }
    }
}
