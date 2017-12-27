using BLL.Mapping;
using BllEntities;
using DAL.Repositories.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Services
{
    public class AttributeLibService : EntityLibService<BllAttribute, AttributeLib, BllAttributeLib, SelectedAttribute, EntityLibMapper<BllAttribute, BllAttributeLib, AttributeService>, AttributeService>
    {
        public AttributeLibService(IUnitOfWork uow) : base(uow, uow.AttributeLibs, uow.SelectedAttributes)
        {
            // this.uow = uow;
        }
    }
}
