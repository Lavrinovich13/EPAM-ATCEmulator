using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ATCEmulator
{
    class Program
    {
        delegate void Call(PhoneNumber n);

        static void Main(string[] args)
        {
            var billingSystem = new BillingSystem("+37529", 100100100);
            var atcStation = new ATC(new List<IPort>() { new Port(), new Port(), new Port(), new Port()});

            //out in methods?
            atcStation.OnCall += billingSystem.AddCall;
            billingSystem.OnContract += atcStation.NewContract;

            var User1 = billingSystem.ConcludeContract(new FreeMinutesTariffPlan());
            var User2 = billingSystem.ConcludeContract(new FreeMinutesTariffPlan());
            var User3 = billingSystem.ConcludeContract(new FreeMinutesTariffPlan());
            var User4 = billingSystem.ConcludeContract(new FreeMinutesTariffPlan());

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
            User1.Answer();
            User1.TerminateConnection();

          var info = billingSystem.GetInfoAboutCalls((x) => { return x.StartedAt.ToShortDateString() == new DateTime(2015, 12, 9).ToShortDateString(); });
        }
    }
}
