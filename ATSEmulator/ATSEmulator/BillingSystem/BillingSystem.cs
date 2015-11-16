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
        protected ICollection<Contract> _CurrentContracts;

        protected ICollection<CallInfo> _NotPayCalls;
        protected ICollection<CallInfo> _PaiedCalls;

        protected ICollection<PhoneNumber> _Debtors;
        protected IDictionary<PhoneNumber, DateTime> _LastPayment;

        public ICollection<ITariffPlan> _TariffPlans { get; protected set; }

        public event EventHandler<ITerminal> OnContract;

        public IEnumerable<Contract> GetContracts { get { return _CurrentContracts.AsEnumerable(); } }

        public BillingSystem(string code, long firstNumber, ILogger logger, ICollection<ITariffPlan> tariffPlans)
        {
            this._Logger = logger;

            this._NumberCode = code;
            this._Number = firstNumber;

            this._OldContracts = new List<Contract>();
            this._CurrentContracts = new List<Contract>();
            this._NotPayCalls = new List<CallInfo>();
            this._PaiedCalls = new List<CallInfo>();

            this._Debtors = new List<PhoneNumber>();
            this._LastPayment = new Dictionary<PhoneNumber, DateTime>();

            this._TariffPlans = tariffPlans;
        }

        public void ConnectToATS(ATS ats)
        {
            ats.OnConnecting += this.IsDebtor;
            ats.OnTerminateCall += this.AddCall;
        }

        public ITerminal ConcludeContract(ITariffPlan tariffPlan)
        {
            if (_TariffPlans.Select(x => tariffPlan.GetType()).Contains(tariffPlan.GetType()))
            {
                var phoneNumber = new PhoneNumber(_NumberCode + _Number.ToString());
                _Number++;

                var dateTime = LocalDateTime.Now;
                var newContract = new Contract(phoneNumber, tariffPlan.GetNewInstance(), dateTime);
                _CurrentContracts.Add(newContract);
                _LastPayment.Add(phoneNumber, dateTime);

                var newTerminal = new Terminal(phoneNumber, _Logger);

                _Logger.WriteToLog("-> Billing system conclude contract");
                _Logger.WriteToLog(ObjectToLogString.ToLogString(newContract));

                OnContract(this, newTerminal);
                return newTerminal;
            }
            return null;
        }

        protected void AddCall(object sender, CallInfo callInfo)
        {
            var currentTariffPlan = 
                this._CurrentContracts
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
            if (_TariffPlans.Select(x => tariffPlan.GetType()).Contains(tariffPlan.GetType()))
            {
                var dateTime = LocalDateTime.Now;

                _Logger.WriteToLog("-> User with number " + phoneNumber.GetValue + " want change tariff " + dateTime.ToShortDateString());

                var currentContract = _CurrentContracts.SingleOrDefault(x => x.Number == phoneNumber);

                if (currentContract != null && currentContract.Date.Month != dateTime.Month)
                {
                    var newContract = new Contract(phoneNumber, tariffPlan, dateTime);
                    _CurrentContracts.Remove(currentContract);
                    _OldContracts.Add(currentContract);

                    _CurrentContracts.Add(newContract);
                    _Logger.WriteToLog("Tariff changed on " + tariffPlan.Name);
                    return true;
                }
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

            var time = LocalDateTime.Now;
            _LastPayment[phoneNumber] =
                new DateTime(time.Year, time.Month , _CurrentContracts.SingleOrDefault(x => x.Number == phoneNumber).Date.Day);

            _Logger.WriteToLog("User with number " + phoneNumber.GetValue + " pay for calls");

            _Debtors.Remove(phoneNumber);
            _NotPayCalls = _NotPayCalls.Where(x => x.Source != phoneNumber).ToList();
        }

        public void DayChanged(object sender, DateTime date)
        {
            _Debtors = _LastPayment
                .Where(x => x.Value.Day <= date.Day && x.Value.AddMonths(2).Month == date.Month)
                .Select(x => x.Key)
                .ToList();
            RefreshTariffs(date.Day);
        }

        protected void RefreshTariffs(int day)
        {
            _CurrentContracts = _CurrentContracts
                .Select(x => x.Date.Day == day ? new Contract(x.Number, x.TariffPlan.GetNewInstance(), x.Date) : x)
                .ToList();
        }

        public bool IsDebtor(PhoneNumber phoneNumber)
        {
            return !_Debtors.Contains(phoneNumber);
        }

        public void ClearEvents()
        {
            this.OnContract = null;
        }
    }
}
