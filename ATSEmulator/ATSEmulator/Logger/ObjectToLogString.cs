using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATSEmulator
{
    public static class ObjectToLogString
    {
        public static string ToLogString(CallInfo callInfo)
        {
            return String.Format("Call: from - {0}, to - {1}, time - {2}, duration(min) - {3}, price - {4}",
                callInfo.Source.GetValue, callInfo.Target.GetValue, callInfo.StartedAt, callInfo.Duration.TotalMinutes, callInfo.Price);
        }

        public static string ToLogString(Response response)
        {
            return String.Format("Respond state is {2} on call from {0} to {1}",
                response.IncomingRequest.SourceNumber.GetValue, response.IncomingRequest.TargetNumber.GetValue, response.State);
        }

        public static string ToLogString(Request request)
        {
            return String.Format("Request on call from {0} to {1}",
                request.SourceNumber.GetValue, request.TargetNumber.GetValue);
        }

        private static string ArrayToString(PhoneNumber[] array)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach(var number in array)
            {
                stringBuilder.Append(number.GetValue + " ");
            }

            return stringBuilder.ToString();
        }

        public static string ToLogString(FavoriteNumbersTariffPlan tariffPlan)
        {
            return String.Format("Tariff: Name - {0}, Description - {1}, Favorite numbers - {2}",
                tariffPlan.Name, tariffPlan.Description, ArrayToString(tariffPlan._FavoriteNumbers));
        }

        public static string ToLogString(FreeMinutesTariffPlan tariffPlan)
        {
            return String.Format("Tariff: Name - {0}, Description - {1}", 
                tariffPlan.Name, tariffPlan.Description);
        }


        public static string ToLogString(Contract contract)
        {
            return String.Format("Contract: Date - {0}, Number - {1}, Name - {2} ",
                contract.Date.ToShortDateString(), contract.Number.GetValue, contract.TariffPlan.Name);
        }
    }
}
