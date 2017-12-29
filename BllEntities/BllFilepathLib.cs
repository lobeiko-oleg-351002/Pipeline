using BllEntities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities
{
    public class BllFilepathLib : IBllEntitySimpleLib<BllFilepath>
    {
        public int Id { get; set; }

        public string FolderName { get; set; }

        public List<BllFilepath> Entities { get; set; }

        public BllFilepathLib()
        {
            Entities = new List<BllFilepath>();
        }
    }
}
