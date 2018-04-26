using DAL.Entities;
using DAL.Mapping.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Mapping
{
    public class ReconcilerLibMapper : IMapper<DalReconcilerLib, ReconcilerLib>
    {
        public DalReconcilerLib MapToDal(ReconcilerLib entity)
        {
            return new DalReconcilerLib
            {
                Id = entity.id,
            };
        }

        public ReconcilerLib MapToOrm(DalReconcilerLib entity)
        {
            var res = new ReconcilerLib();
            res.id = entity.Id;
            return res;
        }
    }
}
