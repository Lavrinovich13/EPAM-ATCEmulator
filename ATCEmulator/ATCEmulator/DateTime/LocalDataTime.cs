using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCEmulator
{
    public static class LocalDateTime
    {
        private static int Counter = 0;
        
        public static DateTime Now
        {
            get
            {
                Counter++;
                DateTime dateTimeNow = DateTime.Now;
                dateTimeNow = dateTimeNow.AddDays(5 * Counter);
                return dateTimeNow;
            }
        }
    }
}
