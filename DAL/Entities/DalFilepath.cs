using DAL.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Entities
{
    public class DalFilepath : IDalEntityWithLibId
    {
        public int Lib_id { get; set ; }
        public int Id { get ; set ; }
        public string Path { get; set; }
    }
}
