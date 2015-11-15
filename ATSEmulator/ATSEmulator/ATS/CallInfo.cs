using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATSEmulator
{
    public class CallInfo
    {
        public PhoneNumber Source { get; protected set; }
        public PhoneNumber Target { get; protected set; }
        public DateTime StartedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public double Price { get; set; }

        public CallInfo (PhoneNumber source, PhoneNumber target)
        {
            this.Source = source;
            this.Target = target;
        }
    }
}
