using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATSEmulator
{
    public interface ITerminal
    {
        PhoneNumber Number { get; }
        Request _ActiveCall { get; }

        event EventHandler OnPluging;
        event EventHandler OnUnPluging;

        event EventHandler<Request> OnRequest;
        event EventHandler<Response> OnResponse;

        event EventHandler<Response> OnEndCall;

        void Plug();
        void UnPlug();

        void SendRequest(PhoneNumber number);

        void Answer();
        void Drop();
        void TerminateConnection();

        void ConnectToPort(IPort port);
    }
}
