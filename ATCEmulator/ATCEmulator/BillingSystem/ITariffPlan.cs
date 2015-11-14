using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCEmulator
{
    public interface ITariffPlan
    {
        string Name { get; }
        string Description { get; }
        double CalculatePriceOfCall(CallInfo call);
    }
}
