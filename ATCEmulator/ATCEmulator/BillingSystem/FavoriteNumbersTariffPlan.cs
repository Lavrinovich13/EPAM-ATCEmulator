using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCEmulator
{
    public class FavoriteNumbersTariffPlan : ITariffPlan
    {
        public PhoneNumber[] _FavoriteNumbers { get; protected set; }
        protected static double _PricePerMinute = 20;

        protected static string _Name = "Favorite";
        protected static string _Description = "Calls on two favorite numbers ar free. And on another 20 per minute.";

        public string Name { get { return _Name; } }
        public string Description { get { return _Description; } }

        public FavoriteNumbersTariffPlan(PhoneNumber[] favoriteNumbers)
        {
            _FavoriteNumbers = new PhoneNumber[2];

            int index = 0;
            if (favoriteNumbers.Count() > _FavoriteNumbers.Count())
            {
                index = favoriteNumbers.Count() - 2;
            }

            favoriteNumbers.CopyTo(_FavoriteNumbers, index);
        }

        public double CalculatePriceOfCall(CallInfo call)
        {
            if (_FavoriteNumbers.Contains(call.Target))
            {
                return 0;
            }
            else
            {
                return call.Duration.TotalMinutes * _PricePerMinute;
            }
        }
    }
}
