﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Entities
{
    public class DalSelectedUserReconciler : DalSelectedEntity
    {
        public bool? IsEventReconciled { get; set; }
    }
}
