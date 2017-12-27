using DAL.Entities;
using DAL.Mapping;
using DAL.Repositories.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories
{
    public class FilepathRepository : Repository<DalFilepath, Filepath, FilepathMapper>, IFilepathRepository, IGetterByLibId<DalFilepath>
    {
        private readonly ServiceDB context;
        public FilepathRepository(ServiceDB context) : base(context)
        {
            this.context = context;
        }

        public IEnumerable<DalFilepath> GetEntitiesByLibId(int id)
        {
            FilepathMapper mapper = new FilepathMapper();
            var elements = context.Set<Filepath>().Where(entity => entity.lib_id == id);
            var retElemets = new List<DalFilepath>();
            foreach (var element in elements)
            {
                retElemets.Add(mapper.MapToDal(element));
            }
            return retElemets;
        }
    }
}
