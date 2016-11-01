using System.Linq;
using System.Text.RegularExpressions;
using BotApplication.Model;

namespace BotApplication.Helpers
{
    public class Validator
    {
        public static string IsSubscrValid(string subscrCode)
        {
            if (string.IsNullOrWhiteSpace(subscrCode))
                return "Извините, я вас не понял";

            if (subscrCode.Length != 11)
                return "Длина номера ЛС должна быть 11 символов";

            if (subscrCode.Any(x => !char.IsNumber(x)))
                return "Номер ЛС должен содержать только цифры";

            return null;
        }

        public static string IsMeterValid(string reading, MeterModel meter)
        {
            if (meter == null)
                return "Извините, я вас не понял";

            if (string.IsNullOrWhiteSpace(reading))
                return "Извините, я вас не понял";

            if (reading.Contains(".") || reading.Contains(","))
            {
                // Показания с точкой
                var regexString = string.Format(@"^\d+([.,]\d{{1,{0}}})?$", meter.Unit);
                var regex = new Regex(regexString);
                if (!regex.IsMatch(reading))
                    return GetWrongReadingMessage(meter.Unit);
            }
            else
            {
                // Целые показания
                var regex = new Regex(@"^[0-9]*$");
                if (!regex.IsMatch(reading))
                    return GetWrongReadingMessage(meter.Unit);
            }

            decimal currentReadingDecimal;
            if (!decimal.TryParse(reading.Replace(".", ","), out currentReadingDecimal))
                return "Извините, я вас не понял";

            if (currentReadingDecimal < meter.BackMeterDec)
                return "Текущее показание не может быть меньше предыдущего";

            return "";
        }
        
        private static string GetWrongReadingMessage(int precision)
        {
            return string.Format("Показание должно содержать только цифры и/или символ . или , и иметь точность {0} символа(-ов)", precision);
        }
    }
}