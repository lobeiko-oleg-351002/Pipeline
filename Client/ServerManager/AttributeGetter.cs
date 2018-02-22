using BllEntities;
using Client.ServerManager.Interface;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ServerManager
{
    public class AttributeGetter : ServerInstance, IAttributeGetter
    {
        public AttributeGetter(IBusinessService server) : base(server)
        {
        }

        public List<BllAttribute> GetAllAttributes()
        {
            try
            {
                return server.GetAllAttributes();
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }
    }
}
