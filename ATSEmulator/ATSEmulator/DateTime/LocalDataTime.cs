using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATSEmulator
{
    public static class LocalDateTime
    {
        private static int _Counter = 0;
        private static int _LastMonth = DateTime.Now.Month;

        public static event EventHandler<DateTime> OnDayChanged;

        public static DateTime Now
        {
            get
            {
                _Counter++;
                DateTime dateTimeNow = DateTime.Now;
                dateTimeNow = dateTimeNow.AddDays(_Counter);

                if (OnDayChanged != null)
                {
                    OnDayChanged(null, dateTimeNow);
                }

                return dateTimeNow;
            }
        }
    }
}
