using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATCEmulator
{
    public interface IPort
    {
        PortState GetState { get; }

        void PlugToTerminal(object sender, EventArgs args);
        void UnPlugFromTerminal(object sender, EventArgs args);

        event EventHandler<Request> OnOutgoingRequest;
        event EventHandler<Request> OnIncomingRequest;

        event EventHandler<Respond> OnOutgoingRespond;
        event EventHandler<Respond> OnIncomingRespond;

        event EventHandler<Respond> OnRequestWasCompleted;
        event EventHandler<Respond> OnTerminateRequest;

        void OutgoingRequest(object sender, Request e);
        void IncomingRequest(Request e);

        void OutgoingResponse(object sender, Respond e);

        void TerminateCall(object sender, Respond e);
        void CallWasCompleted(Respond e);
    }
}
