using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATCEmulator
{
    public class Port : IPort
    {
        private PortState _State = PortState.SwitchedOff;
        public PortState GetState
        {
            get { return _State; }
        }

        public event EventHandler<Request> OnOutgoingCall;
        public event EventHandler<Request> OnIncomingCall;

        public event EventHandler<Respond> OnOutgoingRespond;
        public event EventHandler<Respond> OnIncomingRespond;

        public event EventHandler<Respond> OnEndCall;
        public event EventHandler<Respond> OnTerminateCall;

        public void PlugToTerminal(object sender, EventArgs args)
        {
            if(_State == PortState.SwitchedOff)
            {
                _State = PortState.Free;
            }

            Console.WriteLine("P Порт подключен.");
        }

        public void UnPlugFromTerminal(object sender, EventArgs args)
        {
            if (_State != PortState.SwitchedOff)
            {
                _State = PortState.SwitchedOff;
            }

            Console.WriteLine("P Порт выключен.");
        }

        public void OutgoingCall(object sender, Request e)
        {
            _State = PortState.Busy;

            Console.WriteLine("P Исходящий звонок на порте по номеру {1} к {0}.",
                e.TargetNumber.GetValue, e.SourceNumber.GetValue);

            OnOutgoingCall(this, e);
        }


        public void IncomingCall(Request e)
        {
            Console.WriteLine("P Входящий звонок на порте по номеру {0} от {1}.",
                e.TargetNumber.GetValue, e.SourceNumber.GetValue);
            OnIncomingCall(this, e);
        }

        public void OutgoingRespond(object sender, Respond e)
        {
            if(e.State == RespondState.Answer)
            {
                _State = PortState.Busy;
            }
            Console.WriteLine("P Ответ на входящий звонок от {1} пришел на порт по номеру {0}.  Ответ {2}",
                e.IncomingRequest.TargetNumber.GetValue, e.IncomingRequest.SourceNumber.GetValue, e.State.ToString());
            OnOutgoingRespond(this, e);
        }


        public void IncomingRespond(Respond e)
        {
            if(e.State == RespondState.Answer)
            {
                _State = PortState.Busy;
            }

            Console.WriteLine("P Ответ на исходящий звонок пришел на порт по номеру {1} от {0}.  Ответ {2}",
                e.IncomingRequest.TargetNumber.GetValue, e.IncomingRequest.SourceNumber.GetValue, e.State.ToString());

            OnIncomingRespond(this, e);
        }


        public void EndCall(object sender, Respond e)
        {
            _State = PortState.Free;

            Console.WriteLine("P Завершение исходящий звонока пришло на порт по номеру {0} от {1}.",
                e.IncomingRequest.TargetNumber.GetValue, e.IncomingRequest.SourceNumber.GetValue);

            OnEndCall(this, e);
        }


        public void CallIsCompleted(Respond e)
        {
            _State = PortState.Free;

            Console.WriteLine("P Завершение входящего звонока пришло на порт по номеру {0} от {1}.",
                e.IncomingRequest.TargetNumber.GetValue, e.IncomingRequest.SourceNumber.GetValue);

            OnTerminateCall(this, e);
        }
    }
}
