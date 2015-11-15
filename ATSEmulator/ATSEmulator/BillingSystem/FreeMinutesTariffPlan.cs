using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATSEmulator
{
    public class FreeMinutesTariffPlan : ITariffPlan
    {
        protected static double _FreeMinutes = 100;
        protected static double _PricePerMinute = 15;

        protected static string _Name = "Light";
        protected static string _Description = "First 100 minutes are free. And folowwing 15 per minute.";

        public string Name { get { return _Name; } }
        public string Description { get { return _Description; } }

        protected double _PrivateMinutes;

        public FreeMinutesTariffPlan()
        {
            _PrivateMinutes = _FreeMinutes;
        }

        public double CalculatePriceOfCall(CallInfo call)
        {
            var durationInMinutes = call.Duration.TotalMinutes;

            if (_PrivateMinutes != 0)
            {
                if (durationInMinutes > _PrivateMinutes)
                {
                    durationInMinutes -= _PrivateMinutes;
                    _PrivateMinutes = 0;
                }
                else
                {
                    _PrivateMinutes -= durationInMinutes;
                    return 0;
                }
            }

            return durationInMinutes * _PricePerMinute;
        }

        public ITariffPlan GetNewInstance()
        {
            return new FreeMinutesTariffPlan();
        }
    }
}
