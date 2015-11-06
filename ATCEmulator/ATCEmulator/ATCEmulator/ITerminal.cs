using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATCEmulator
{
    public interface ITerminal
    {
        PhoneNumber Number { get; }

        event EventHandler Pluging;
        event EventHandler UnPluging;

        void Plug();
        void UnPlug();
    }
}
