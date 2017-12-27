using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Entities.Interface
{
    public interface IDalSelectedEntity : IDalEntityWithLibId
    {
        int Entity_id { get; set; }
    }
}
