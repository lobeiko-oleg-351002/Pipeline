using BllEntities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities
{
    public static class BllEntityComparer
    {
        public static int GetItemIndex(IBllEntity item, List<IBllEntity> array)  // returns index
        {
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i].Id == item.Id)
                {
                    return i;
                }
            }
            return -1;
        }

    }
}
