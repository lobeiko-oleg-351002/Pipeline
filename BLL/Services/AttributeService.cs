using BLL.Mapping;
using BLL.Services.Interface;
using BllEntities;
using DAL.Entities;
using DAL.Repositories.Interface;
using ORM;

namespace BLL.Services
{
    public class AttributeService : Service<BllAttribute, DalAttribute, Attribute, AttributeMapper>, IAttributeService
    {

        public AttributeService(IUnitOfWork uow) : base(uow, uow.Attributes)
        {

        }
    }
}
