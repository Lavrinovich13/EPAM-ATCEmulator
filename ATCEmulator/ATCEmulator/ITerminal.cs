using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATCEmulator
{
    public interface ITerminal
    {
        PhoneNumber Number { get; }

        event EventHandler OnPluging;
        event EventHandler OnUnPluging;

        event EventHandler<Request> OnCall;
        event EventHandler<Respond> OnRespond;

        event EventHandler<Respond> OnEndCall;

        void Plug();
        void UnPlug();

        void Call(PhoneNumber number);

        void AnswerOnCall(Request request);
        void RejectCall(Request request);

        void ReactOnIncomingCall(object sender, Request request);
        void Respond(object sender, Respond e);
        void CallIsCompleted(object sender, EventArgs e);
    }
}
