using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCEmulator
{
    public class ATC
    {
        private static Random _Random = new Random();

        protected IDictionary<IPort, bool> _Ports;
        protected IDictionary<PhoneNumber, IPort> _RoutingPorts;
        protected ICollection<ITerminal> _Terminals;

        protected ICollection<CallInfo> _ConnectingCalls;
        protected ICollection<CallInfo> _ActiveCalls;

        public ATC(ICollection<IPort> ports)
        {
            this._RoutingPorts = new Dictionary<PhoneNumber, IPort>();
            this._Terminals = new List<ITerminal>();
            this._Ports = ports.ToDictionary(port => port, port => true);

            this._ConnectingCalls = new List<CallInfo>();
            this._ActiveCalls = new List<CallInfo>();
        }

        public ITerminal ConcludeContract()
        {
            ITerminal terminal = null; 
            var freePort = _Ports.FirstOrDefault(x => x.Value == true).Key;

            if (freePort != null)
            {
                terminal = new Terminal(new PhoneNumber("100-100-" + _Random.Next(1, 100).ToString()));
                _RoutingPorts.Add(terminal.Number, freePort);
                _Ports[freePort] = false;
                _Terminals.Add(terminal);
                ConnectPortAndTerminal(freePort, terminal);

              RegisterOnPortEvent(freePort);
            }

            Console.WriteLine("A Новый терминал с номером {0}.", terminal.Number.GetValue);

            return terminal;
        }

        protected void ConnectPortAndTerminal(IPort port, ITerminal terminal)
        {
            terminal.OnPluging += port.PlugToTerminal;
            terminal.OnUnPluging += port.UnPlugFromTerminal;

            terminal.OnCall += port.OutgoingCall;
            terminal.OnRespond += port.OutgoingRespond;
            terminal.OnEndCall += port.EndCall;

            port.OnIncomingCall += terminal.ReactOnIncomingCall;
            port.OnIncomingRespond += terminal.Respond;
            port.OnCompletedCall += terminal.TerminateCall;
        }

        protected void RegisterOnPortEvent(IPort port)
        {
            port.OnOutgoingCall += this.ProcessRequest;
            port.OnOutgoingRespond += this.ProcessRespond;
            port.OnEndCall += this.ProcessEndOfCall;
        }

        private void ProcessEndOfCall(object sender, Respond e)
        {
            var senderPort = _RoutingPorts[e.IncomingRequest.SourceNumber];
            var targetPort = _RoutingPorts[e.IncomingRequest.TargetNumber];

            var callInfo = _ActiveCalls.FirstOrDefault(x => x.Source == e.IncomingRequest.SourceNumber
               && x.Target == e.IncomingRequest.TargetNumber);

            if(callInfo != null)
            {
                callInfo.SetDuration(DateTime.Now);
                _ActiveCalls.Remove(callInfo);

                Console.WriteLine("A Заверешение звонка пришело от {0} к {1}.",
                    e.IncomingRequest.SourceNumber.GetValue, e.IncomingRequest.TargetNumber.GetValue);

                targetPort.CallIsCompleted(e);
            }
        }

        protected void ProcessRequest(object sender, Request e)
        {
            var senderPort = _RoutingPorts[e.SourceNumber];
            var targetPort = _RoutingPorts[e.TargetNumber];

            if (targetPort != null && targetPort.GetState == PortState.Free)
            {
                var callInfo = new CallInfo(e.SourceNumber, e.TargetNumber);
                this._ConnectingCalls.Add(callInfo);

                Console.WriteLine("A Звонок пришел на АТС от {0} к {1}.",
                    e.SourceNumber.GetValue, e.TargetNumber.GetValue);

                targetPort.IncomingCall(e);
            }
            else
            {
                Console.WriteLine("A Звонок пришедший на АТС от {0} к {1} отклонен",
                    e.SourceNumber.GetValue, e.TargetNumber.GetValue);

                senderPort.IncomingRespond(new Respond(RespondState.Reject, e));
            }
        }

        protected void ProcessRespond(object sender, Respond e)
        {
            var senderPort = _RoutingPorts[e.IncomingRequest.SourceNumber];
            var targetPort = _RoutingPorts[e.IncomingRequest.TargetNumber];

            var callInfo = _ConnectingCalls.FirstOrDefault(x => x.Source == e.IncomingRequest.SourceNumber 
                && x.Target == e.IncomingRequest.TargetNumber);

            if (callInfo != null)
            {
                _ConnectingCalls.Remove(callInfo);

                if (e.State == RespondState.Answer)
                {
                    callInfo.SetStartTime(DateTime.Now);
                    _ActiveCalls.Add(callInfo);

                    Console.WriteLine("A Ответ на звонок от {0} к {1} пришел на АТС. Ответ {2}.",
                        e.IncomingRequest.SourceNumber.GetValue, e.IncomingRequest.TargetNumber.GetValue, e.State.ToString());

                    senderPort.IncomingRespond(e);
                }
                else
                {
                    Console.WriteLine("A Ответ на звонок от {0} к {1} пришел на АТС. Ответ {2}.",
                        e.IncomingRequest.SourceNumber.GetValue, e.IncomingRequest.TargetNumber.GetValue, e.State.ToString());

                    senderPort.IncomingRespond(e);
                }
            }
        }
    }
}
