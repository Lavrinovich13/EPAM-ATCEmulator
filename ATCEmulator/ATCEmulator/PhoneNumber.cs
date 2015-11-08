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

        public override bool Equals(object obj)
        {
            if (obj is PhoneNumber)
            {
                return this == (obj as PhoneNumber);
            }
            else
            {
                return false;
            }

        }

        public static bool operator ==(PhoneNumber ph1, PhoneNumber ph2)
        {
            return ph1._Value.SequenceEqual(ph2._Value);
        }

        public static bool operator !=(PhoneNumber ph1, PhoneNumber ph2)
        {
            return !ph1._Value.SequenceEqual(ph2._Value);
        }

        public override int GetHashCode()
        {
            return _Value.GetHashCode();
        }
    }
}
