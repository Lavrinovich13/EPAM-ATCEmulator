using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATSEmulator
{
    public class Port : IPort
    {
        protected ILogger _Logger;
        private PortState _State = PortState.SwitchedOff;
        public PortState GetState
        {
            get { return _State; }
        }

        public event EventHandler<Request> OnOutgoingRequest;
        public event EventHandler<Request> OnIncomingRequest;

        public event EventHandler<Response> OnOutgoingResponse;

        public event EventHandler<Response> OnTerminateRequest;
        public event EventHandler<Response> OnRequestWasCompleted;

        public Port(ILogger logger)
        {
            this._Logger = logger;
        }

        public void PlugToTerminal(object sender, EventArgs args)
        {
            if(_State == PortState.SwitchedOff)
            {
                _State = PortState.Free;
                RegisterOnTerminalEvents(sender as ITerminal);
            }

            _Logger.WriteToLog("-> Port on number " + (sender as ITerminal).Number.GetValue + " plug");
        }

        public void UnPlugFromTerminal(object sender, EventArgs args)
        {
            if (_State != PortState.SwitchedOff)
            {
                _State = PortState.SwitchedOff;

                UnsubscribeFromTerminalEvents(sender as ITerminal);
            }

            _Logger.WriteToLog("-> Port on number " + (sender as ITerminal).Number.GetValue + " unplug");
        }

        public void OutgoingRequest(object sender, Request request)
        {
            _State = PortState.Busy;

            _Logger.WriteToLog("-> Port on number " + (sender as ITerminal).Number.GetValue + " change state in " + _State);

            OnOutgoingRequest(this, request);
        }

        public void IncomingRequest(Request request)
        {
            _State = PortState.Busy;

            _Logger.WriteToLog("-> Port on number " + request.TargetNumber.GetValue + " change state in " + _State);

            OnIncomingRequest(this, request);
        }

        public void OutgoingResponse(object sender, Response response)
        {
            _State = response.State == ResponseState.Answer ? PortState.Busy : PortState.Free;
            _Logger.WriteToLog("-> Port on number " + (sender as ITerminal).Number.GetValue + " change state in " + _State);
            OnOutgoingResponse(this, response);
        }

        public void TerminateCall(object sender, Response response)
        {
            _State = PortState.Free;

            _Logger.WriteToLog("-> Port on number " + (sender as ITerminal).Number.GetValue + " change state in " + _State);

            OnTerminateRequest(this, response);
        }

        public void CallWasCompleted(Response response)
        {
            _State = PortState.Free;

            //??
            _Logger.WriteToLog("-> Target port change state in " + _State);

            OnRequestWasCompleted(this, response);
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
            this.OnOutgoingRequest = null;
            this.OnOutgoingResponse = null;
            this.OnRequestWasCompleted = null;
            this.OnTerminateRequest = null;
        }
    }
}
