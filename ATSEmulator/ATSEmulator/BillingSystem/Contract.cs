using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATSEmulator
{
    public class Contract
    {
        public DateTime Date { get; protected set; }
        public PhoneNumber Number { get; protected set; }
        public ITariffPlan TariffPlan { get; protected set; }

        public Contract(PhoneNumber number, ITariffPlan tariffPlan, DateTime date)
        {
            this.Number = number;
            this.TariffPlan = tariffPlan;
            this.Date = date;
        }
    }
}
