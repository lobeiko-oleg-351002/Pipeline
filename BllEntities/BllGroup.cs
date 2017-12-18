using BllEntities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities
{
    public class BllGroup : IBllEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
