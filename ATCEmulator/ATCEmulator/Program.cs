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
            User1.Plug();

            var User2 = AtcStation.ConcludeContract();
            User2.Plug();

            var User3 = AtcStation.ConcludeContract();
            User3.Plug();

            var User4 = AtcStation.ConcludeContract();
            User4.Plug();

            Call call = User1.Call;
            IAsyncResult result = call.BeginInvoke(User2.Number, null, null);
            Call call2 = User3.Call;
            IAsyncResult result1 = call2.BeginInvoke(User4.Number, null, null);

            Thread.Sleep(5000);
        }
    }
}
