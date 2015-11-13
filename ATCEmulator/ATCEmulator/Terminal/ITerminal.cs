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

        event EventHandler<Request> OnRequest;
        event EventHandler<Respond> OnResponse;

        event EventHandler<Respond> OnEndCall;

        void Plug();
        void UnPlug();

        void SendRequest(PhoneNumber number);

        void Answer();
        void Drop();
        void TerminateConnection();

        void RegisterOnPortEvents(IPort port);
    }
}
