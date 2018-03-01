using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ServerManager
{
    public static class DateTimeHelper
    {
        public static bool AreDatesLayInRange(DateTime statusTime, DateTime serverTime, int rangeInDays)
        {
            if (AddWorkingDays(statusTime, rangeInDays).CompareTo(serverTime) < 0)
            {
                return true;
            }
            return false;
        }

        private static DateTime AddWorkingDays(DateTime date, int days)
        {
            int overallDays = 0;
            for (int i = 0; i < days;)
            {
                overallDays++;
                DateTime d = date.AddDays(overallDays);
                if ((d.DayOfWeek != DayOfWeek.Saturday) && (d.DayOfWeek != DayOfWeek.Sunday))
                {
                    i++;
                }
                d.AddDays(-overallDays);
            }
            return date.AddDays(overallDays);
        }
    }
}
