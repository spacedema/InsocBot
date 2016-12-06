using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BotApplication.Model;
using Newtonsoft.Json;

namespace BotApplication.Helpers
{
    [Serializable]
    public static class ApiHelper
    {
        private static string getUrl = "http://lk.insoc.ru/MeterByPribors/getapi/";
        private static string postUrl = "http://lk.insoc.ru/MeterByPribors/saveapi/";

        public async static Task<MeterFormModel> Get(string subscrCode, string partner)
        {
            using (var client = new HttpClient())
            {
                var query = string.Format("?subscr={0}&org={1}", subscrCode, partner);
                var data = await client.GetStringAsync(getUrl + query);
                if (data == null) 
                    return null;

                var meter = JsonConvert.DeserializeObject(data, typeof(MeterFormModel));
                return meter as MeterFormModel;
            }
        }

        public async static Task<string> Save(string jObject)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent(jObject, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync(postUrl, content);
                    var str = await result.Content.ReadAsStringAsync();
                    var jObj = JsonConvert.DeserializeObject<Response>(str);
                    return jObj.Success ? "Показания принты!" : jObj.Message;
                }
            }
            catch (Exception)
            {
                return "Произошла ошибка. Попробуйте позже.";
            }
        }

        public class Response
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }
    }
}