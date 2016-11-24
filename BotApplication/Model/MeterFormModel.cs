using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotApplication.Model
{
    [Serializable]
    public class MeterFormModel
    {
        /// <summary>
        /// Идентификатор Л/С
        /// </summary>
        public int SubscrId { get; set; }
        /// <summary>
        /// Дата снятия показания
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Показания счетчиков
        /// </summary>
        public IEnumerable<MeterModel> Meters { get; set; }

        public string Result { get; set; }
        public string Message { get; set; }

        public string Name { get; set; }
    }
}