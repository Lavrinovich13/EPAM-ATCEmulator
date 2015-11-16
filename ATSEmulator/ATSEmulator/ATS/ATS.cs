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

        protected IDictionary<IPort, bool> _PortsIsAvailable;
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
            this._PortsIsAvailable = ports.ToDictionary(port => port, port => true);

            this._ConnectingCalls = new List<CallInfo>();
            this._ActiveCalls = new List<CallInfo>();
        }

        public void ConnectToBillingSystem(IBillingSystem billingSystem)
        {
            billingSystem.OnContract += this.NewContract;
        }

        protected void NewContract(object sender, ITerminal terminal)
        {
            var freePort = _PortsIsAvailable.FirstOrDefault(x => x.Value == true).Key;

            if (freePort != null)
            {
                _RoutingPorts.Add(terminal.Number, freePort);
                _PortsIsAvailable[freePort] = false;
                _Terminals.Add(terminal);

                terminal.ConnectToPort(freePort);
                RegisterOnPortEvent(freePort);

                _Logger.WriteToLog("-> New terminal on number " + terminal.Number.GetValue + " get port");
            }
        }

        public void RegisterOnPortEvent(IPort port)
        {
            port.OnOutgoingRequest += this.ProcessRequest;
            port.OnOutgoingResponse += this.ProcessResponse;
            port.OnTerminateRequest += this.ProcessEndOfCall;
            port.OnUnplaged += this.TerminateAllConnections;
        }

        protected void ProcessRequest(object sender, Request request)
        {
            var senderPort = _RoutingPorts[request.SourceNumber];
            var targetPort = _RoutingPorts[request.TargetNumber];

            if (OnConnecting != null && !OnConnecting(request.SourceNumber))
            {
                RejectRequest(request, ResponseState.NotEnoughtMoney, senderPort);
            }
            else if (targetPort == null)
            {
                RejectRequest(request, ResponseState.TargetNotExist, senderPort);
            }
            else if (targetPort.GetState == PortState.Busy)
            {
                RejectRequest(request, ResponseState.Busy, senderPort);
            }
            else if (targetPort.GetState == PortState.SwitchedOff)
            {
                RejectRequest(request, ResponseState.Offline, senderPort);
            }
            else
            {
                var callInfo = new CallInfo(request.SourceNumber, request.TargetNumber);
                this._ConnectingCalls.Add(callInfo);
                AcceptRequest(request, targetPort);
            }
        }

        protected void AcceptRequest(Request request, IPort port)
        {
            _Logger.WriteToLog("-> ATS process");
            _Logger.WriteToLog(ObjectToLogString.ToLogString(request));

            port.IncomingRequest(request);
        }

        protected void RejectRequest(Request request, ResponseState responseState, IPort port)
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

            var callInfo = _ConnectingCalls
                .FirstOrDefault(x => x.Source == response.IncomingRequest.SourceNumber
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

        protected void ProcessEndOfCall(object sender, Response response)
        {
            var senderPort = _RoutingPorts[response.IncomingRequest.SourceNumber];
            var targetPort = _RoutingPorts[response.IncomingRequest.TargetNumber];

            var callInfo = _ActiveCalls.FirstOrDefault(x => x.Source == response.IncomingRequest.SourceNumber
               && x.Target == response.IncomingRequest.TargetNumber);

            if (callInfo != null)
            {
                callInfo.Duration = LocalTimeSpan.Duration(callInfo.StartedAt, LocalDateTime.Now);

                _Logger.WriteToLog("-> ATS process end on");
                _Logger.WriteToLog(ObjectToLogString.ToLogString(response.IncomingRequest));

                TerminateCall(_ActiveCalls, response.IncomingRequest, (sender as IPort));

                OnTerminateCall(this, callInfo);
            }
        }

        protected void TerminateAllConnections(object sender, Request request)
        {
            TerminateCall(_ConnectingCalls, request, (sender as IPort));
            TerminateCall(_ActiveCalls, request, (sender as IPort));
        }

        protected void TerminateCall(ICollection<CallInfo> calls, Request request, IPort sender)
        {
            var terminateCall = 
                calls.SingleOrDefault(x => x.Source == request.SourceNumber && x.Target == request.TargetNumber);

            if (terminateCall != null)
            {
                calls.Remove(terminateCall);

                var senderPort = _RoutingPorts[request.SourceNumber];
                var targetPort = _RoutingPorts[request.TargetNumber];

                
                    if (senderPort == sender)
                    {
                        targetPort.CallWasCompleted(null);
                    }
                    else
                    {
                        senderPort.CallWasCompleted(null);
                    }
            }
        }

        public void ClearEvents()
        {
            this.OnTerminateCall = null;
            this.OnConnecting = null;
        }
    }
}
