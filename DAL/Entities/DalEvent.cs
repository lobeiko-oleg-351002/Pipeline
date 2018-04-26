using DAL.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Entities
{
    public class DalEvent : IDalEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Status_lib_id { get; set; }

        public int Filepath_lib_id { get; set; }

        public int Attribute_lib_id { get; set; }

        public int Receiver_lib_id { get; set; }

        public int Sender_id { get; set; }

        public int Type_id { get; set; }

        public int? Approver_id { get; set; }

        public DateTime Date { get; set; }

        public bool? IsApproved { get; set; }

        public int? Reconciler_lib_id { get; set; }
    }
}
