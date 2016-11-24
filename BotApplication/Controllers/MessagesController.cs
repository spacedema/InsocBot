using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BotApplication.Helpers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace BotApplication.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        internal static IDialog<MeterReadings> MakeRoot()
        {
            return Chain.From(() => FormDialog.FromForm(MeterReadings.MakeForm))
                .Do(async (context, order) =>
                {
                    var reply = string.Empty;
                    try
                    {
                        // ReSharper disable once UnusedVariable
                        var completed = await order;
                        var result = await ApiHelper.Save(JsonConvert.SerializeObject(completed));
                        await context.PostAsync(result);
                    }
                    catch (FormCanceledException<MeterReadings> e)
                    {
                        reply = e.InnerException == null ? "Вы прервали операцию, попробуем позже!" : "Извините, произошла ошибка. Попробуйте позже.";
                    }
                    if (!string.IsNullOrEmpty(reply))
                        await context.PostAsync(reply);
                });
        }

        [ResponseType(typeof (void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            if (activity != null)
            {
                //if the user types certain messages, quit all dialogs and start over
                if (activity.Text != null)
                {
                    var msg = activity.Text.ToLower().Trim();
                    if (StopWords.Contains(msg))
                    {
                        //This is where the conversation gets reset!
                        activity.GetStateClient().BotState.DeleteStateForUser(activity.ChannelId, activity.From.Id);
                    }
                }

                switch (activity.GetActivityType())
                {
                    case ActivityTypes.Message:
                    {
                        try
                        {
                            await Conversation.SendAsync(activity, MakeRoot);
                        }
                        catch (Exception)
                        {
                            // ignore
                        }
                        
                        break;
                    }

                    default:
                        Trace.TraceError("Unknown activity type ignored: {0}", activity.GetActivityType());
                        break;
                }
            }
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        static readonly string[] StopWords = { "reset", "restart", "exit", "cancel", "quit", "done", "stop", "отмена", "заного", "заново", "занова", "выход", "рестарт", "отменить", "стоп"};
    }
}