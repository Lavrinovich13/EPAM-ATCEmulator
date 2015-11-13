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

        public event EventHandler<Request> OnOutgoingRequest;
        public event EventHandler<Request> OnIncomingRequest;

        public event EventHandler<Respond> OnOutgoingRespond;
        public event EventHandler<Respond> OnIncomingRespond;

        public event EventHandler<Respond> OnTerminateRequest;
        public event EventHandler<Respond> OnRequestWasCompleted;

        public void PlugToTerminal(object sender, EventArgs args)
        {
            if(_State == PortState.SwitchedOff)
            {
                _State = PortState.Free;
                RegisterOnTerminalEvents(sender as ITerminal);
            }

            Console.WriteLine("P Порт подключен.");
        }

        public void UnPlugFromTerminal(object sender, EventArgs args)
        {
            if (_State != PortState.SwitchedOff)
            {
                _State = PortState.SwitchedOff;

                UnsubscribeFromTerminalEvents(sender as ITerminal);
            }

            Console.WriteLine("P Порт выключен.");
        }

        public void OutgoingRequest(object sender, Request e)
        {
            _State = PortState.Busy;

            Console.WriteLine("P Исходящий звонок на порте по номеру {1} к {0}.",
                e.TargetNumber.GetValue, e.SourceNumber.GetValue);

            OnOutgoingRequest(this, e);
        }

        public void IncomingRequest(Request e)
        {
            _State = PortState.Busy;
            Console.WriteLine("P Входящий звонок на порте по номеру {0} от {1}.",
                e.TargetNumber.GetValue, e.SourceNumber.GetValue);
            OnIncomingRequest(this, e);
        }

        public void OutgoingResponse(object sender, Respond e)
        {
            if(e.State == RespondState.Answer)
            {
                _State = PortState.Busy;
            }
            Console.WriteLine("P Ответ на входящий звонок от {1} пришел на порт по номеру {0}.  Ответ {2}",
                e.IncomingRequest.TargetNumber.GetValue, e.IncomingRequest.SourceNumber.GetValue, e.State.ToString());
            OnOutgoingRespond(this, e);
        }

        public void TerminateCall(object sender, Respond e)
        {
            _State = PortState.Free;

            Console.WriteLine("P Завершение исходящий звонока пришло на порт по номеру {0} от {1}.",
                e.IncomingRequest.TargetNumber.GetValue, e.IncomingRequest.SourceNumber.GetValue);

            OnTerminateRequest(this, e);
        }

        public void CallWasCompleted(Respond e)
        {
            _State = PortState.Free;

            Console.WriteLine("P Завершение входящего звонока пришло на порт по номеру {0} от {1}.",
                e.IncomingRequest.TargetNumber.GetValue, e.IncomingRequest.SourceNumber.GetValue);

            OnRequestWasCompleted(this, e);
        }

        protected void RegisterOnTerminalEvents(ITerminal terminal)
        {
            //it must be virtual

            terminal.OnUnPluging += this.UnPlugFromTerminal;

            terminal.OnRequest += this.OutgoingRequest;
            terminal.OnResponse += this.OutgoingResponse;
            terminal.OnEndCall += this.TerminateCall;
        }

        protected void UnsubscribeFromTerminalEvents(ITerminal terminal)
        {
            terminal.OnUnPluging -= this.UnPlugFromTerminal;

            terminal.OnRequest -= this.OutgoingRequest;
            terminal.OnResponse -= this.OutgoingResponse;
            terminal.OnEndCall -= this.TerminateCall;
        }

        public void ClearEvents()
        {
            //too much events

            this.OnIncomingRequest = null;
            this.OnIncomingRespond = null;
            this.OnOutgoingRequest = null;
            this.OnOutgoingRespond = null;
            this.OnRequestWasCompleted = null;
            this.OnTerminateRequest = null;
        }
    }
}
