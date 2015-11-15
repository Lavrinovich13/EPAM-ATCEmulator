using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATSEmulator
{
    public class BillingSystem : IBillingSystem
    {
        protected ILogger _Logger;
        protected string _NumberCode;
        protected long _Number;

        protected ICollection<Contract> _OldContracts;
        protected ICollection<Contract> _NewContracts;

        protected ICollection<CallInfo> _NotPayCalls;
        protected ICollection<CallInfo> _PaiedCalls;

        protected IList<PhoneNumber> _Debtors;
        protected IDictionary<PhoneNumber, DateTime> _LastPayment;

        public IList<ITariffPlan> _TariffPlans { get; protected set; }
        public event EventHandler<ITerminal> OnContract; 

        public BillingSystem(string code, long firstNumber, ILogger logger)
        {
            //TariffPlans
            this._Logger = logger;

            this._NumberCode = code;
            this._Number = firstNumber;

            this._OldContracts = new List<Contract>();
            this._NewContracts = new List<Contract>();
            this._NotPayCalls = new List<CallInfo>();
            this._PaiedCalls = new List<CallInfo>();

            this._Debtors = new List<PhoneNumber>();
            this._LastPayment = new Dictionary<PhoneNumber, DateTime>();
        }

        public ITerminal ConcludeContract(ITariffPlan tariffPlan)
        {
            var phoneNumber = new PhoneNumber(_NumberCode + _Number.ToString());
            _Number++;

            var dateTime = LocalDateTime.Now;
            var newContract = new Contract(phoneNumber, tariffPlan.GetNewInstance(), dateTime);
            _NewContracts.Add(newContract);
            _LastPayment.Add(phoneNumber, dateTime);

            var newTerminal = new Terminal(phoneNumber, _Logger);

            _Logger.WriteToLog("-> Billing system conclude contract");
            _Logger.WriteToLog(ObjectToLogString.ToLogString(newContract));

            OnContract(this, newTerminal);

            return newTerminal;
        }

        public void AddCall(object sender, CallInfo callInfo)
        {
            var currentTariffPlan = 
                this._NewContracts
                .Where(x => x.Number == callInfo.Source)
                .OrderBy(x => x.Date)
                .Last()
                .TariffPlan;

            callInfo.Price = currentTariffPlan.CalculatePriceOfCall(callInfo);

            _Logger.WriteToLog("-> Billing system get new call");
            _Logger.WriteToLog(ObjectToLogString.ToLogString(callInfo));

            _NotPayCalls.Add(callInfo);
        }

        public bool ChangeTariffPlan(PhoneNumber phoneNumber, ITariffPlan tariffPlan)
        {
            var dateTime = LocalDateTime.Now;
            _Logger.WriteToLog("-> User with number " + phoneNumber.GetValue + " want change tariff");
            var currentContract = _NewContracts.SingleOrDefault(x => x.Number == phoneNumber);

            if(currentContract != null && currentContract.Date.Month != dateTime.Month)
            {
                 var newContract = new Contract(phoneNumber, tariffPlan, dateTime);
                 _NewContracts.Remove(currentContract);
                 _OldContracts.Add(currentContract);

                 _NewContracts.Add(newContract);
                 _Logger.WriteToLog("Tariff changed on " + tariffPlan.Name);
                 return true;
            }
            _Logger.WriteToLog("Tariff was not changed");
            return false;
        }

        public IList<CallInfo> GetInfoAboutCalls(Func<CallInfo, bool> predicate)
        {
            return _NotPayCalls.Concat(_PaiedCalls).Where(x => predicate(x)).ToList();
        }

        public double GetDebtOnCurrentMoment(PhoneNumber phoneNumber)
        {
            return _NotPayCalls
                .Where(x => x.Source == phoneNumber)
                .Sum(x => x.Price);
        }

        public void Pay(PhoneNumber phoneNumber)
        {
            var calls = _NotPayCalls.Where(x => x.Source == phoneNumber).ToList();
            _PaiedCalls = _PaiedCalls.Concat(calls).ToList();
            _Debtors.Remove(phoneNumber);

            var time = LocalDateTime.Now;
            _LastPayment[phoneNumber] =
                new DateTime(time.Year, time.Month , _NewContracts.SingleOrDefault(x => x.Number == phoneNumber).Date.Day);

            _NotPayCalls = _NotPayCalls.Where(x => x.Source != phoneNumber).ToList();
        }

        public void ClearEvents()
        {
            this.OnContract = null;
        }

        public void DayChanged(object sender, DateTime date)
        {
            if (date.Month == 1)
            { }
            _Debtors = _LastPayment
                .Where(x => x.Value.Day <= date.Day && ((x.Value.Month >= 11 ? x.Value.Month - 12 : x.Value.Month) + 2 == date.Month))
                .Select(x => x.Key)
                .ToList();
            RefreshTariffs(date.Day);
        }

        protected void RefreshTariffs(int day)
        {
            _NewContracts = _NewContracts
                .Select(x => x.Date.Day == day ? new Contract(x.Number, x.TariffPlan.GetNewInstance(), x.Date) : x)
                .ToList();
        }

        public bool IsDebtor(PhoneNumber phoneNumber)
        {
            return !_Debtors.Contains(phoneNumber);
        }
    }
}
