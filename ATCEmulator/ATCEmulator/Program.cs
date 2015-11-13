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
            ATC AtcStation = new ATC(new List<IPort>() { new Port(), new Port(), new Port(), new Port() });

            var User1 = AtcStation.ConcludeContract();

            var User2 = AtcStation.ConcludeContract();

            var User3 = AtcStation.ConcludeContract();

            var User4 = AtcStation.ConcludeContract();

            User1.SendRequest(User2.Number);
            User2.Answer();
            User2.TerminateConnection();

            User3.SendRequest(User1.Number);

        }
    }
}
