using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCEmulator
{
    public class Request
    {
        public PhoneNumber SourceNumber { get; protected set; }
        public PhoneNumber TargetNumber { get; protected set; }

        public Request(PhoneNumber sourceNumber, PhoneNumber targetNumber)
        {
            this.SourceNumber = sourceNumber;
            this.TargetNumber = targetNumber;
        }
    }
}
