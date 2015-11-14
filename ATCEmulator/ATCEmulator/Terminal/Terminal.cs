using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ATCEmulator
{
    public class Terminal : ITerminal
    {
        protected ILogger _Logger;

        protected PhoneNumber _PhoneNumber;
        protected Request _ActiveCall;

        protected bool _IsPlugToPort = false;

        public Terminal(PhoneNumber number, ILogger logger)
        {
            this._PhoneNumber = number;
            this._Logger = logger;
        }

        public event EventHandler OnPluging;
        public event EventHandler OnUnPluging;

        public event EventHandler<Request> OnRequest;
        public event EventHandler<Respond> OnResponse;

        public event EventHandler<Respond> OnEndCall;

        public void Plug()
        {
            _IsPlugToPort = true;
            OnPluging(this, null);
        }

        public void UnPlug()
        {
            _IsPlugToPort = false;
            OnUnPluging(this, null);
        }

        public PhoneNumber Number
        {
            get { return _PhoneNumber; }
        }

        public void SendRequest(PhoneNumber targetNumber)
        {
            if(_ActiveCall != null)
            {
                this.TerminateConnection();
            }
            _ActiveCall = new Request(_PhoneNumber, targetNumber);

            _Logger.WriteToLog("-> Send request");
            _Logger.WriteToLog(ObjectToLogString.ToLogString(_ActiveCall));

            OnRequest(this, _ActiveCall);
        }

        public void Answer()
        {
            SendResponse(_ActiveCall, RespondState.Answer);
        }

        public void Drop()
        {
            var request = _ActiveCall;
            _ActiveCall = null;

            SendResponse(request, RespondState.Reject);
        }

        protected void SendResponse(Request request, RespondState state)
        {
            var respond = new Respond(state, request);

            _Logger.WriteToLog("-> Incoming response");
            _Logger.WriteToLog(ObjectToLogString.ToLogString(respond));

            OnResponse(this, respond);
        }

        public void TerminateConnection()
        {
            OnEndCall(this, new Respond(RespondState.Reject, _ActiveCall));
            _ActiveCall = null;

            _Logger.WriteToLog("-> Call terminate by " + _PhoneNumber.GetValue);
        }

        protected void CallWasTerminated(object sender, Respond respond)
        {
            _ActiveCall = null;

            _Logger.WriteToLog("-> Call terminate in " + _PhoneNumber.GetValue);
        }

        public void RegisterOnPortEvents(IPort port)
        {
            //it must be virtual

            port.OnIncomingRequest += (sender, request) => { _ActiveCall = request; };
            port.OnRequestWasCompleted += this.CallWasTerminated;
        }

        public void ClearEvents()
        {
            //or protected?

            this.OnEndCall = null;
            this.OnRequest = null;
            this.OnResponse = null;
        }
    }
}
