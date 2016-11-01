using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotApplication.Model
{
    [Serializable]
    public class MeterModel
    {
//        public string PlaceHolderMessageByCurrentMeter()
//        {
//            string unitString = "";
//            for (var i = 0; i < Unit; i++)
//            {
//                unitString += "0";
//            }
//            if (Unit > 0)
//                return "123 или 123." + unitString;
//            else
//                return "123";
//        }
//
//        public string CurrentMeterTooltip()
//        {
//            return "Вводимые данные должны быть в формате " + (Unit > 0 ? PlaceHolderMessageByCurrentMeter() : " 123");
//        }
//
//        public string GetCurrentMeter()
//        {
//            if (CurrentMeterDec.HasValue && CurrentMeterDec > 0)
//                return GetDecimalToString(CurrentMeterDec);
//            else
//                return "";
//        }
//
//        public string GetBackMeter()
//        {
//            if (BackMeterDec > 0)
//                return GetDecimalToString(BackMeterDec);
//            else
//                return "";
//        }
//
//        public string GetConsumption()
//        {
//            if (Consumption > 0)
//                return GetDecimalToString(Consumption);
//            else
//                return "";
//        }
//
//
        private string GetDecimalToString(Decimal? dec)
        {
            if (!dec.HasValue)
                return string.Empty;

            return GetDecimalToString(dec.Value);
        }

        private string GetDecimalToString(Decimal dec)
        {
            return dec.ToString("F" + Unit, new System.Globalization.CultureInfo("en-US"));
        }

        private string GetDecimalToStringDb(Decimal? dec)
        {
            if (!dec.HasValue)
                return string.Empty;

            return GetDecimalToStringDb(dec.Value);
        }

        private string GetDecimalToStringDb(Decimal dec)
        {
            return dec.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Возвращается идентификатор для вывода на сайте
        /// </summary>
        public string WebID
        {
            get
            {
                return string.Format("{0}_{1}", MeterId, F_Registr_Pts);
            }
        }

        /// <summary>
        /// Идентификтаор счетчика
        /// </summary>
        public string MeterId { get; set; }
        /// <summary>
        /// Тип счетчика, Номер счетчика, Место установки, для двухтактных электосчетсиков (день, ночь)
        /// </summary>
        
        public string MeterDysplayName { get; set; }
        /// <summary>
        /// Предыдущее показание
        /// </summary>
        public decimal BackMeterDec
        {
            get
            {
                return Decimal.Parse(BackMeterStr.Replace(",", "."), new System.Globalization.CultureInfo("en-US"));
            }
            set
            {
                BackMeterStr = GetDecimalToString(value);
            }
        }

        public string BackMeterStr { get; set; }
        
        /// <summary>
        /// Текущее показание
        /// </summary>
        public string CurrentMeterStr { get; set; }

        public decimal? CurrentMeterDec
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CurrentMeterStr))
                    return null;
                return Decimal.Parse(CurrentMeterStr.Replace(",", "."), new System.Globalization.CultureInfo("en-US"));
            }
            set
            {
                CurrentMeterStr = value > 0 ? GetDecimalToStringDb(value) : null;
            }
        }

        /// <summary>
        /// Потребление
        /// </summary>
        public decimal Consumption { get; set; }
        
        /// <summary>
        /// Дата последнего снятия показания
        /// </summary>
        public DateTime LastDate { get; set; }
        /// <summary>
        /// Имя услуги
        /// </summary>
        //[MapField("C_Service")]
        public string ServiceName { get; set; }

        public int F_Registr_Pts { get; set; }

        public int N_Period { get; set; }
        /// <summary>
        /// Дата последнего ввода показания(если был в этом месяце)
        /// </summary>
        public DateTime? D_MR_Date { get; set; }

        /// <summary>
        /// Точность значение (кол-во знаков после запятой)
        /// </summary>
        public int Unit { get; set; }

        #region Overrides of Object

        public override string ToString()
        {
            return MeterDysplayName.Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Replace("<", "").Replace(">", "");
        }

        #endregion
    }
}