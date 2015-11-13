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

            terminal.RegisterOnPortEvents(port);
            terminal.Plug();
        }

        protected void RegisterOnPortEvent(IPort port)
        {
            port.OnOutgoingRequest += this.ProcessRequest;
            port.OnOutgoingRespond += this.ProcessRespond;
            port.OnTerminateRequest += this.ProcessEndOfCall;
        }

        private void ProcessEndOfCall(object sender, Respond e)
        {
            var senderPort = _RoutingPorts[e.IncomingRequest.SourceNumber];
            var targetPort = _RoutingPorts[e.IncomingRequest.TargetNumber];

            var callInfo = _ActiveCalls.FirstOrDefault(x => x.Source == e.IncomingRequest.SourceNumber
               && x.Target == e.IncomingRequest.TargetNumber);

            if(callInfo != null)
            {
                callInfo.SetDuration(LocalTimeSpan.Duration(callInfo.StartedAt, LocalDateTime.Now));
                _ActiveCalls.Remove(callInfo);

                Console.WriteLine("A Заверешение звонка пришло от {0} к {1}.",
                    e.IncomingRequest.SourceNumber.GetValue, e.IncomingRequest.TargetNumber.GetValue);

                if ((sender as IPort) == targetPort)
                {
                    senderPort.CallWasCompleted(e);
                }
                else
                {
                    targetPort.CallWasCompleted(e);
                }
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

                targetPort.IncomingRequest(e);
            }
            else
            {
                Console.WriteLine("A Звонок пришедший на АТС от {0} к {1} отклонен",
                    e.SourceNumber.GetValue, e.TargetNumber.GetValue);

                senderPort.CallWasCompleted(new Respond(RespondState.Reject, e));
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
                    callInfo.SetStartTime(LocalDateTime.Now);
                    _ActiveCalls.Add(callInfo);

                    Console.WriteLine("A Ответ на звонок от {0} к {1} пришел на АТС. Ответ {2}.",
                        e.IncomingRequest.SourceNumber.GetValue, e.IncomingRequest.TargetNumber.GetValue, e.State.ToString());
                }
                else
                {
                    Console.WriteLine("A Ответ на звонок от {0} к {1} пришел на АТС. Ответ {2}.",
                        e.IncomingRequest.SourceNumber.GetValue, e.IncomingRequest.TargetNumber.GetValue, e.State.ToString());
                    senderPort.CallWasCompleted(e);
                }
            }
        }
    }
}
