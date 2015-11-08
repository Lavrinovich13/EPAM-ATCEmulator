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

        event EventHandler<Request> OnOutgoingCall;
        event EventHandler<Request> OnIncomingCall;

        event EventHandler<Respond> OnOutgoingRespond;
        event EventHandler<Respond> OnIncomingRespond;

        event EventHandler<Respond> OnCompletedCall;
        event EventHandler<Respond> OnEndCall;

        void OutgoingCall(object sender, Request e);
        void IncomingCall(Request e);

        void OutgoingRespond(object sender, Respond e);
        void IncomingRespond(Respond e);

        void EndCall(object sender, Respond e);
        void CallIsCompleted(Respond e);
    }
}
