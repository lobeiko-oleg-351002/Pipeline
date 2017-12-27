using DAL.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Entities
{
    public class DalAttribute : IDalEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
