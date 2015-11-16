using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ATSEmulator
{
    class Program
    {
        delegate void Call(PhoneNumber n);

        static void Main(string[] args)
        {
            var logger = new ConsoleLogger();
            var tariffPlans = new List<ITariffPlan> { new FavoriteNumbersTariffPlan(), new FreeMinutesTariffPlan()};
            var billingSystem = new BillingSystem("+37529", 100100100, logger, tariffPlans);
            var atsStation = new ATS(new List<IPort>() { new Port(logger), new Port(logger), new Port(logger), new Port(logger)}, logger);

            billingSystem.ConnectToATS(atsStation);
            atsStation.ConnectToBillingSystem(billingSystem);

            LocalDateTime.OnDayChanged += billingSystem.DayChanged;

            var User1 = billingSystem.ConcludeContract(new FreeMinutesTariffPlan());
            User1.Plug();
            var User2 = billingSystem.ConcludeContract(new FavoriteNumbersTariffPlan(new[] { User1.Number }));
            User2.Plug();
            var User3 = billingSystem.ConcludeContract(new FreeMinutesTariffPlan());
            User3.Plug();
            var User4 = billingSystem.ConcludeContract(new FreeMinutesTariffPlan());
            User4.Plug();

            User1.SendRequest(User2.Number);
            User2.Answer();

            User3.SendRequest(User2.Number);
            User4.SendRequest(User3.Number);
            User2.TerminateConnection();

            User1.SendRequest(User2.Number);
            User2.Answer();
            User2.TerminateConnection();

            User3.Answer();
            User4.TerminateConnection();

            User3.SendRequest(User1.Number);
            User1.Drop();

            User2.SendRequest(User1.Number);
            User1.Answer();
            User1.TerminateConnection();

            User2.SendRequest(User4.Number);
            User4.Answer();
            User2.TerminateConnection();

            billingSystem.ChangeTariffPlan(User2.Number, new FreeMinutesTariffPlan());
            billingSystem.ChangeTariffPlan(User2.Number, new FavoriteNumbersTariffPlan(new[] { User3.Number }));

            var info = billingSystem.GetInfoAboutCalls((x) => { return x.Source == User2.Number || x.Target == User2.Number; });
            var t = billingSystem.GetDebtOnCurrentMoment(User2.Number);
        }
    }
}
