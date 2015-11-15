using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATSEmulator
{
    public class ATS
    {
        protected ILogger _Logger;

        protected IDictionary<IPort, bool> _Ports;
        protected IDictionary<PhoneNumber, IPort> _RoutingPorts;
        protected ICollection<ITerminal> _Terminals;

        protected ICollection<CallInfo> _ConnectingCalls;
        protected ICollection<CallInfo> _ActiveCalls;

        public event EventHandler<CallInfo> OnTerminateCall;
        public event Func<PhoneNumber, bool> OnConnecting;

        public ATS(ICollection<IPort> ports, ILogger logger)
        {
            this._Logger = logger;

            this._RoutingPorts = new Dictionary<PhoneNumber, IPort>();
            this._Terminals = new List<ITerminal>();
            this._Ports = ports.ToDictionary(port => port, port => true);

            this._ConnectingCalls = new List<CallInfo>();
            this._ActiveCalls = new List<CallInfo>();
        }

        public void NewContract(object sender, ITerminal terminal)
        {
            var freePort = _Ports.FirstOrDefault(x => x.Value == true).Key;

            if (freePort != null)
            {
                _RoutingPorts.Add(terminal.Number, freePort);
                _Ports[freePort] = false;
                _Terminals.Add(terminal);

                _Logger.WriteToLog("-> New terminal on number " + terminal.Number.GetValue + " get port");
                ConnectPortAndTerminal(freePort, terminal);

                RegisterOnPortEvent(freePort);
            }
        }

        protected void ConnectPortAndTerminal(IPort port, ITerminal terminal)
        {
            terminal.OnPluging += port.PlugToTerminal;

            terminal.RegisterOnPortEvents(port);
        }

        protected void RegisterOnPortEvent(IPort port)
        {
            port.OnOutgoingRequest += this.ProcessRequest;
            port.OnOutgoingResponse += this.ProcessResponse;
            port.OnTerminateRequest += this.ProcessEndOfCall;
        }

        private void ProcessEndOfCall(object sender, Response response)
        {
            var senderPort = _RoutingPorts[response.IncomingRequest.SourceNumber];
            var targetPort = _RoutingPorts[response.IncomingRequest.TargetNumber];

            var callInfo = _ActiveCalls.FirstOrDefault(x => x.Source == response.IncomingRequest.SourceNumber
               && x.Target == response.IncomingRequest.TargetNumber);

            if (callInfo != null)
            {
                callInfo.Duration = LocalTimeSpan.Duration(callInfo.StartedAt, LocalDateTime.Now);
                _ActiveCalls.Remove(callInfo);

                _Logger.WriteToLog("-> ATS process end on");
                _Logger.WriteToLog(ObjectToLogString.ToLogString(response.IncomingRequest));


                if ((sender as IPort) == targetPort)
                {
                    senderPort.CallWasCompleted(response);
                }
                else
                {
                    targetPort.CallWasCompleted(response);
                }

                OnTerminateCall(this, callInfo);
            }
        }

        public void ClearEvents()
        {
            this.OnTerminateCall = null;
        }

        protected void ProcessRequest(object sender, Request request)
        {
            var senderPort = _RoutingPorts[request.SourceNumber];
            var targetPort = _RoutingPorts[request.TargetNumber];

            if (OnConnecting != null && !OnConnecting(request.SourceNumber))
            {
                RejectResponse(request, ResponseState.NotEnoughtMoney, senderPort);
            }
            else if (targetPort == null)
            {
                RejectResponse(request, ResponseState.TargetNotExist, senderPort);
            }
            else if (targetPort.GetState == PortState.Busy)
            {
                RejectResponse(request, ResponseState.Busy, senderPort);
            }
            else if (targetPort.GetState == PortState.SwitchedOff)
            {
                RejectResponse(request, ResponseState.Offline, senderPort);
            }
            else
            {
                var callInfo = new CallInfo(request.SourceNumber, request.TargetNumber);
                this._ConnectingCalls.Add(callInfo);
                SendRequest(request, targetPort);
            }
        }

        protected void SendRequest(Request request, IPort port)
        {
            _Logger.WriteToLog("-> ATS process");
            _Logger.WriteToLog(ObjectToLogString.ToLogString(request));

            port.IncomingRequest(request);
        }

        protected void RejectResponse(Request request, ResponseState responseState, IPort port)
        {
            var response = new Response(responseState, request);
            _Logger.WriteToLog("-> ATS reject");
            _Logger.WriteToLog(ObjectToLogString.ToLogString(response));
            port.CallWasCompleted(response);
        }

        protected void ProcessResponse(object sender, Response response)
        {
            var senderPort = _RoutingPorts[response.IncomingRequest.SourceNumber];
            var targetPort = _RoutingPorts[response.IncomingRequest.TargetNumber];

            var callInfo = _ConnectingCalls.FirstOrDefault(x => x.Source == response.IncomingRequest.SourceNumber
                && x.Target == response.IncomingRequest.TargetNumber);

            _Logger.WriteToLog("-> ATS process");
            _Logger.WriteToLog(ObjectToLogString.ToLogString(response));

            if (callInfo != null)
            {
                _ConnectingCalls.Remove(callInfo);

                if (response.State == ResponseState.Answer)
                {
                    callInfo.StartedAt = LocalDateTime.Now;
                    _ActiveCalls.Add(callInfo);
                }
                else
                {
                    senderPort.CallWasCompleted(response);
                }
            }
        }
    }
}
