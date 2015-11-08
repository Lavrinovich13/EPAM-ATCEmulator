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

        protected bool _IsOnline = false;
        public Terminal(PhoneNumber number)
        {
            this._PhoneNumber = number;
        }

        public event EventHandler OnPluging;

        public event EventHandler OnUnPluging;

        public event EventHandler<Request> OnCall;

        public event EventHandler<Respond> OnRespond;

        public event EventHandler<Respond> OnEndCall;

        public void Plug()
        {
            OnPluging(this, null);
        }

        public void UnPlug()
        {
            OnUnPluging(this, null);
        }

        public PhoneNumber Number
        {
            get { return _PhoneNumber; }
        }

        public void Call(PhoneNumber targetNumber)
        {
            if (!_IsOnline)
            {
                _IsOnline = true;

                Console.WriteLine("T Абонент {0} вызывает {1}. Состояние занят {2}.",
                    _PhoneNumber.GetValue, targetNumber.GetValue, _IsOnline);

                OnCall(this, new Request(_PhoneNumber, targetNumber));
            }
        }

        public void ReactOnIncomingCall(object sender, Request request)
        {
            if (!_IsOnline)
            {
                AnswerOnCall(request);
            }
            else
            {
                RejectCall(request);
            }
        }

        public void AnswerOnCall(Request request)
        {
            _IsOnline = true;

            MakeRespond(request, RespondState.Answer);
        }

        public void RejectCall(Request request)
        {
            MakeRespond(request, RespondState.Reject);
        }

        protected void MakeRespond(Request request, RespondState state)
        {
            var respond = new Respond(state, request);

            Console.WriteLine("T На звонок от {0} {1} ответил {3}. Состояние занят {2}.",
                request.SourceNumber.GetValue, request.TargetNumber.GetValue, _IsOnline, state.ToString());

            OnRespond(this, respond);
        }

        public void Respond(object sender, Respond e)
        {
            //TODO delete it and replace functianality
            if (e.State == RespondState.Answer)
            {
                Thread.Sleep(1000);
                _IsOnline = false;

                Console.WriteLine("T Прервал звонок от {0} к {1}. Состояние занят {2}.",
                    e.IncomingRequest.SourceNumber.GetValue, e.IncomingRequest.TargetNumber.GetValue, _IsOnline);

                OnEndCall(this, e);
            }
            else
            {

                _IsOnline = false;
            }
        }

        public void CallIsCompleted(object sender, EventArgs e)
        {
            _IsOnline = false;

            Console.WriteLine("T Звонок к {0} окончен. Состояние занят {1}.",
                _PhoneNumber.GetValue, _IsOnline);
        }
    }
}
