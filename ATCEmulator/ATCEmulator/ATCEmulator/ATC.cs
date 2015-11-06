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

        private IDictionary<IPort, bool> _Ports;
        private IDictionary<PhoneNumber, IPort> _RoutingPorts;
        private ICollection<ITerminal> _Terminals;

        public ATC(ICollection<IPort> ports)
        {
            this._RoutingPorts = new Dictionary<PhoneNumber, IPort>();
            this._Terminals = new List<ITerminal>();
            this._Ports = ports.ToDictionary(port => port, port => true);
        }

        public ITerminal ConcludeContract()
        {
            var terminal = new Terminal(new PhoneNumber("100-100-" + _Random.Next(1, 100).ToString()));
            var freePort = _Ports.FirstOrDefault(x => x.Value == true).Key;

            _RoutingPorts.Add(terminal.Number, freePort);
            _Ports[freePort] = false;
            _Terminals.Add(terminal);

            RegisterTerminalOnEvent(terminal);
            RegisterPortOnEvent(freePort);

            return terminal;
        }

        private void RegisterPortOnEvent(IPort freePort)
        {
            throw new NotImplementedException();
        }

        private void RegisterTerminalOnEvent(Terminal terminal)
        {
            throw new NotImplementedException();
        }
    }
}
