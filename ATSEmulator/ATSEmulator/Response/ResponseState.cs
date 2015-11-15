using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATSEmulator
{
    public enum ResponseState
    {
        Answer,
        Reject,
        Busy,
        NotEnoughtMoney,
        TargetNotExist,
        Offline
    }
}
