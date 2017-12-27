using DAL.Entities;
using DAL.Mapping;
using DAL.Repositories.Interface;
using ORM;


namespace DAL.Repositories
{
    public class AttributeRepository : Repository<DalAttribute, Attribute, AttributeMapper>, IAttributeRepository
    {
        private readonly ServiceDB context;
        public AttributeRepository(ServiceDB context) : base(context)
        {
            this.context = context;
        }
    }
}
