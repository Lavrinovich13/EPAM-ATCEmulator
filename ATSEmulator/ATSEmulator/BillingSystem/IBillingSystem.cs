using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATSEmulator
{
    public interface IBillingSystem
    {
        ITerminal ConcludeContract(ITariffPlan tariffPlan);

        void AddCall(object sender, CallInfo callInfo);

        bool ChangeTariffPlan(PhoneNumber phoneNumber, ITariffPlan tariffPlan);

        IList<CallInfo> GetInfoAboutCalls(Func<CallInfo, bool> predicate);

        double GetDebtOnCurrentMoment(PhoneNumber phoneNumber);

        void Pay(PhoneNumber phoneNumber);

        void ClearEvents();

        void DayChanged(object sender, DateTime date);

        bool IsDebtor(PhoneNumber phoneNumber);
    }
}
