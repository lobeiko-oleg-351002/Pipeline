using DAL.Entities;
using DAL.Mapping.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Mapping
{
    public class SelectedUserReconcilerMapper : IMapper<DalSelectedUserReconciler, SelectedUserReconciler>
    {
        public DalSelectedUserReconciler MapToDal(SelectedUserReconciler entity)
        {
            return new DalSelectedUserReconciler
            {
                Id = entity.id,
                Lib_id = entity.lib_id,
                Entity_id = entity.entity_id,
                IsEventReconciled = entity.isEventReconciled,
            };
        }

        public SelectedUserReconciler MapToOrm(DalSelectedUserReconciler entity)
        {
            return new SelectedUserReconciler
            {
                id = entity.Id,
                entity_id = entity.Entity_id,
                lib_id = entity.Lib_id,
                isEventReconciled = entity.IsEventReconciled,
            };
        }
    }
}
