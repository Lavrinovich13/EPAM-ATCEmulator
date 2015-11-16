using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATSEmulator
{
    public interface IBillingSystem
    {
        event EventHandler<ITerminal> OnContract;

        ITerminal ConcludeContract(ITariffPlan tariffPlan);

        bool ChangeTariffPlan(PhoneNumber phoneNumber, ITariffPlan tariffPlan);

        IList<CallInfo> GetInfoAboutCalls(Func<CallInfo, bool> predicate);

        double GetDebtOnCurrentMoment(PhoneNumber phoneNumber);

        void Pay(PhoneNumber phoneNumber);

        void DayChanged(object sender, DateTime date);

        bool IsDebtor(PhoneNumber phoneNumber);

        void ConnectToATS(ATS ats);

        void ClearEvents();
    }
}
