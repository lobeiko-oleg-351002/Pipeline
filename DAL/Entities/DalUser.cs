using DAL.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Entities
{
    public class DalUser : IDalEntity
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public int Group_id { get; set; }

        public string Fullname { get; set; }

        public int EventTypeLib_id { get; set; }

        public int StatusLib_id { get; set; }
    }
}
