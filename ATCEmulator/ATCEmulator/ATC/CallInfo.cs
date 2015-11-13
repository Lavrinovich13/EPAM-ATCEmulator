using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCEmulator
{
    public class CallInfo
    {
        private static Random random = new Random();
        public PhoneNumber Source { get; protected set; }
        public PhoneNumber Target { get; protected set; }
        public DateTime StartedAt { get; protected set; }
        public TimeSpan Duration { get; protected set; }

        public CallInfo (PhoneNumber source, PhoneNumber target)
        {
            this.Source = source;
            this.Target = target;
            this.StartedAt = new DateTime();
            this.Duration = new TimeSpan(0,0,0);
        }

        public void SetStartTime(DateTime startTime)
        {
            this.StartedAt = startTime;
        }

        public void SetDuration(TimeSpan duration)
        {
            this.Duration = duration;
        }
    }
}
