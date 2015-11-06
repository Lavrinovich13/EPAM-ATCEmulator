using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCEmulator
{
    class Terminal : ITerminal
    {
        private PhoneNumber _PhoneNumber;
        public Terminal(PhoneNumber number)
        {
            this._PhoneNumber = number;
        }

        public event EventHandler Pluging;

        public event EventHandler UnPluging;

        public void Plug()
        {
            Pluging(this, new EventArgs());
        }

        public void UnPlug()
        {
            UnPluging(this, new EventArgs());
        }

        public PhoneNumber Number
        {
            get { return _PhoneNumber; }
        }
    }
}
