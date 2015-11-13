using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCEmulator
{
    public static class LocalTimeSpan
    {
        public static TimeSpan Duration(DateTime start, DateTime end)
        {
            return new TimeSpan(0, end.Millisecond / 10, 0); 
        }
    }
}
