using BllEntities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities
{
    public class BllUser : IBllEntity
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public BllGroup Group { get; set; }

        public string Fullname { get; set; }

        public bool CreateRights { get; set; }

        public bool ChangeRights { get; set; }
    }
}
