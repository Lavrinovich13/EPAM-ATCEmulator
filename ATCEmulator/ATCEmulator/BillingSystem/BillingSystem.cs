using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCEmulator
{
    public class BillingSystem : IBillingSystem
    {
        protected string _NumberCode;
        protected long _Number;

        protected IList<Contract> _Contracts;
        protected IList<CallInfo> _Calls;

        public event EventHandler<ITerminal> OnContract; 

        public BillingSystem(string code, long firstNumber)
        {
            this._NumberCode = code;
            this._Number = firstNumber;

            this._Contracts = new List<Contract>();
            this._Calls = new List<CallInfo>();
        }

        public ITerminal ConcludeContract(ITariffPlan tariffPlan)
        {
            var phoneNumber = new PhoneNumber(_NumberCode + _Number.ToString());
            _Number++;

            var newContract = new Contract(phoneNumber, tariffPlan, LocalDateTime.Now);
            _Contracts.Add(newContract);

            var newTerminal = new Terminal(phoneNumber);
            OnContract(this, newTerminal);

            return newTerminal;
        }

        public void AddCall(object sender, CallInfo callInfo)
        {
            var currentTariffPlan = 
                this._Contracts
                .Where(x => x.Number == callInfo.Source)
                .OrderBy(x => x.Date)
                .Last()
                .TariffPlan;

            callInfo.Price = currentTariffPlan.CalculatePriceOfCall(callInfo);

            _Calls.Add(callInfo);
        }

        public bool ChangeTariffPlan(PhoneNumber phoneNumber, ITariffPlan tariffPlan)
        {
            //maybe void?
            var dateTime = LocalDateTime.Now;

            if(_Contracts.Where(x => x.Number == phoneNumber).Max(x => x.Date).Month != dateTime.Month)
            {
                 var newContract = new Contract(phoneNumber, tariffPlan, dateTime);
                 return true;
            }
            return false;
        }

        public IList<CallInfo> GetInfoAboutCalls(Func<CallInfo, bool> predicate)
        {
            return _Calls.Where(x => predicate(x)).ToList();
        }

        public void ClearEvents()
        {
            this.OnContract = null;
        }
    }
}
