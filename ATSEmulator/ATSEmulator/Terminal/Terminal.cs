using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ATSEmulator
{
    public class Terminal : ITerminal
    {
        protected ILogger _Logger;

        protected PhoneNumber _PhoneNumber;
        public Request _ActiveCall { get; protected set; }

        protected bool _IsPlugToPort = false;

        public Terminal(PhoneNumber number, ILogger logger)
        {
            this._PhoneNumber = number;
            this._Logger = logger;
        }

        public event EventHandler OnPluging;
        public event EventHandler OnUnPluging;

        public event EventHandler<Request> OnRequest;
        public event EventHandler<Response> OnResponse;

        public event EventHandler<Response> OnEndCall;

        public void Plug()
        {
            _IsPlugToPort = true;
            OnPluging(this, null);
        }

        public void UnPlug()
        {
            _IsPlugToPort = false;
            OnUnPluging(this, null);
            _ActiveCall = null;
        }

        public PhoneNumber Number
        {
            get { return _PhoneNumber; }
        }

        public void SendRequest(PhoneNumber targetNumber)
        {
            if (OnRequest != null)
            {
                if (_ActiveCall != null)
                {
                    this.TerminateConnection();
                }
                _ActiveCall = new Request(_PhoneNumber, targetNumber);

                _Logger.WriteToLog("-> Send request");
                _Logger.WriteToLog(ObjectToLogString.ToLogString(_ActiveCall));

                OnRequest(this, _ActiveCall);
            }
        }

        public void Answer()
        {
            if (_ActiveCall != null)
            {
                SendResponse(_ActiveCall, ResponseState.Answer);
            }
        }

        public void Drop()
        {
            if (_ActiveCall != null)
            {
                var request = _ActiveCall;
                _ActiveCall = null;

                SendResponse(request, ResponseState.Reject);
            }
        }

        protected void SendResponse(Request request, ResponseState state)
        {
            if (OnResponse != null)
            {
                var respond = new Response(state, request);

                _Logger.WriteToLog("-> Response on incoming call");
                _Logger.WriteToLog(ObjectToLogString.ToLogString(respond));

                OnResponse(this, respond);
            }
        }

        public void TerminateConnection()
        {
            if (_ActiveCall != null)
            {
                _Logger.WriteToLog("-> Call terminate by " + _PhoneNumber.GetValue);
                OnEndCall(this, new Response(ResponseState.Reject, _ActiveCall));
                _ActiveCall = null;
            }
        }

        protected void CallWasTerminated(object sender, Response respond)
        {
            _ActiveCall = null;

            _Logger.WriteToLog("-> Call terminate in " + _PhoneNumber.GetValue);
        }

        public void ConnectToPort(IPort port)
        {
            this.OnPluging += port.PlugToTerminal;
            port.OnIncomingRequest += (sender, request) => { _ActiveCall = request; };
            port.OnRequestWasCompleted += this.CallWasTerminated;
        }

        public void ClearEvents()
        {
            this.OnPluging = null;
            this.OnUnPluging = null;
            this.OnEndCall = null;
            this.OnRequest = null;
            this.OnResponse = null;
        }
    }
}
