using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Client.Misc.IndicationService
{
    public class IconSet
    {
        public readonly Icon BaseIcon;
        public readonly Icon NewEventIcon;
        public readonly Icon ServerOfflineIcon;

        public IconSet(Icon Base, Icon NewEvent, Icon ServerOffline)
        {
            BaseIcon = Base;
            NewEventIcon = NewEvent;
            ServerOfflineIcon = ServerOffline;
        }
    }
}
