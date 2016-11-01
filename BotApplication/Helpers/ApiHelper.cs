using System;
using System.Net.Http;
using System.Threading.Tasks;
using BotApplication.Model;
using Newtonsoft.Json;

namespace BotApplication.Helpers
{
    [Serializable]
    public static class ApiHelper
    {
        public async static Task<MeterFormModel> GetUser(string subscrCode)
        {
            var url = "https://lk.insoc.ru/MeterByPribors/getapi/" + subscrCode;
            var client = new HttpClient();
            var data = await client.GetStringAsync(url);
            if (data != null)
            {
                var meter = JsonConvert.DeserializeObject(data, typeof (MeterFormModel));
                return meter as MeterFormModel;
            }

            return null;
        }

        public static void SaveReading(MeterReadings reading)
        {

        }
    }
}