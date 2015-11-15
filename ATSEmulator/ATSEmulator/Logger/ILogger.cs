using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATSEmulator
{
    public interface ILogger
    {
        void WriteToLog(string log);
    }
}
