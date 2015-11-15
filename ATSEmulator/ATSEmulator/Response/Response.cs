
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATSEmulator
{
    public class Response
    {
        public ResponseState State { get; protected set; }
        public Request IncomingRequest { get; protected set; }

        public Response(ResponseState state, Request incomingRequest)
        {
            this.State = state;
            this.IncomingRequest = incomingRequest;
        }
    }
}
