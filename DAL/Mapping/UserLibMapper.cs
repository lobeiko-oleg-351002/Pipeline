using DAL.Entities.Interface;
using DAL.Mapping.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Mapping
{
    public class UserLibMapper : IMapper<DalUserLib, UserLib>
    {
        public DalUserLib MapToDal(UserLib entity)
        {
            return new DalUserLib
            {
                Id = entity.id,
            };
        }

        public UserLib MapToOrm(DalUserLib entity)
        {
            var res = new UserLib();
            res.id = entity.Id;
            return res;
        }
    }
}
