using BllEntities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities
{
    public class BllEvent : IBllEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public BllStatusLib StatusLib { get; set; }

        public BllFilepathLib FilepathLib { get; set; }

        public BllUser Sender { get; set; }

        public BllEventType Type { get; set; }

        public BllAttributeLib AttributeLib { get; set; }

        public BllUserLib RecieverLib { get; set; }

        public DateTime Date { get; set; }
    }
}
