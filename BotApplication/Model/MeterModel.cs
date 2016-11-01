using System;

namespace BotApplication.Model
{
    [Serializable]
    public class MeterModel
    {
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
        // ReSharper disable once InconsistentNaming
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
        public decimal? BackMeterDec
        {
            get
            {
                return Decimal.Parse(BackMeterStr.Replace(",", "."), new System.Globalization.CultureInfo("en-US"));
            }
            set
            {
                BackMeterStr = value < 0 ? null : GetDecimalToStringDb(value);
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
                CurrentMeterStr = value < 0? null : GetDecimalToStringDb(value);
            }
        }

        /// <summary>
        /// Потребление
        /// </summary>
        public decimal Consumption { get; set; }
        
        /// <summary>
        /// Дата последнего снятия показания
        /// </summary>
        public DateTime? LastDate { get; set; }
        /// <summary>
        /// Имя услуги
        /// </summary>
        public string ServiceName { get; set; }

        // ReSharper disable once InconsistentNaming
        public int F_Registr_Pts { get; set; }

        // ReSharper disable once InconsistentNaming
        public int N_Period { get; set; }
        /// <summary>
        /// Дата последнего ввода показания(если был в этом месяце)
        /// </summary>
        // ReSharper disable once InconsistentNaming
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