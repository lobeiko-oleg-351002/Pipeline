using DAL.Entities;
using DAL.Mapping.Interface;
using ORM;

namespace DAL.Mapping
{
    public class AttributeMapper : IAttributeMapper
    {
        public DalAttribute MapToDal(Attribute entity)
        {
            return new DalAttribute
            {
                Id = entity.id,
                Name = entity.name
            };
        }

        public Attribute MapToOrm(DalAttribute entity)
        {
            return new Attribute
            {
                id = entity.Id,
                name = entity.Name,
            };
        }
    }
}
