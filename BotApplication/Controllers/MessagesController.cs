using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;

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
                        var completed = await order;
                        await context.PostAsync("Показания приняты");
                    }
                    catch (FormCanceledException<MeterReadings> e)
                    {
                        if (e.InnerException == null)
                        {
                            reply = "Вы прервали операцию, попробуем позже!";
                        }
                        else
                        {
                            reply = "Извините, произошла ошибка. Попробуйте позже.";
                        }
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
                string msg = activity.Text.ToLower().Trim();
                if (StopWords.Contains(msg))
                {
                    //This is where the conversation gets reset!
                    activity.GetStateClient().BotState.DeleteStateForUser(activity.ChannelId, activity.From.Id);
                    //await Conversation.SendAsync(activity, MakeRoot);
                }

                switch (activity.GetActivityType())
                {
                    case ActivityTypes.Message:
                    {
                        try
                        {
                            await Conversation.SendAsync(activity, MakeRoot);
                        }
                        catch (Exception ex)
                        {
                            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "botErrorSendAsync" + ".txt"), ex.ToString());
                        }
                        
                        break;
                    }

                    case ActivityTypes.ConversationUpdate:
                    case ActivityTypes.ContactRelationUpdate:
                    case ActivityTypes.Typing:
                    case ActivityTypes.DeleteUserData:
                    default:
                        Trace.TraceError(string.Format("Unknown activity type ignored: {0}", activity.GetActivityType()));
                        break;
                }
            }
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        static readonly string[] StopWords = { "reset", "restart", "exit", "cancel", "quit", "done", "отмена", "заного", "заново", "занова", "выход", "рестарт", "отменить" };
    }
}