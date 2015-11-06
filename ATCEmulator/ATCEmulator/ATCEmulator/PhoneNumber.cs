using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCEmulator
{
    public class PhoneNumber
    {
        private string _Value;
        public string GetValue { get { return _Value; } }

        public PhoneNumber(string phoneNumber)
        {
            this._Value = phoneNumber;
        }
    }
}
