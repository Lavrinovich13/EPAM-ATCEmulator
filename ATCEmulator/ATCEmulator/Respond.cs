
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCEmulator
{
    public class Respond
    {
        public RespondState State { get; protected set; }
        public Request IncomingRequest { get; protected set; }

        public Respond(RespondState state, Request incomingRequest)
        {
            this.State = state;
            this.IncomingRequest = incomingRequest;
        }
    }
}
