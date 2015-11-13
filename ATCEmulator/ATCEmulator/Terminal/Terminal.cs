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
        protected PhoneNumber _PhoneNumber;
        protected Request _ActiveCall;

        protected bool _IsPlugToPort = false;

        public Terminal(PhoneNumber number)
        {
            this._PhoneNumber = number;
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
            Console.WriteLine("T Абонент {0} вызывает {1}. ",
                _PhoneNumber.GetValue, targetNumber.GetValue);

            _ActiveCall = new Request(_PhoneNumber, targetNumber);

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

            Console.WriteLine("T На звонок от {0} {1} ответил {2}. ",
                request.SourceNumber.GetValue, request.TargetNumber.GetValue, state.ToString());

            OnResponse(this, respond);
        }

        public void TerminateConnection()
        {
            OnEndCall(this, new Respond(RespondState.Reject, _ActiveCall));
            _ActiveCall = null;
            Console.WriteLine("T Звонок к {0} окончен. ",
                _PhoneNumber.GetValue);
        }

        protected void CallWasTerminated(object sender, Respond respond)
        {
            _ActiveCall = null;
            Console.WriteLine("T Звонок к {0} окончен. ",
                _PhoneNumber.GetValue);
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
