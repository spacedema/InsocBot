﻿using System;
using System.Linq;
using System.Threading.Tasks;
using BotApplication.Helpers;
using BotApplication.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;

namespace BotApplication
{
    [Template(TemplateUsage.NotUnderstood, "Я не понимаю \"{0}\".", "Попробуйте снова, я не понял \"{0}\".")]
    [Serializable]
    public class MeterReadings
    {
        [Prompt("Введите 11-ти значный номер лицевого счета")]
        [Describe("Номер ЛС")]
        [Terms("Номер ЛС")]
        public string SubscrCode;

        [Describe("cчетчик для ввода показаний")]
        [Terms("cчетчик для ввода показаний")]
        public string MeterId;

        [Prompt("Введите показание")]
        [Describe("Показание")]
        [Terms("Показание")]
        public string Reading;

        public MeterFormModel User;

        public static IForm<MeterReadings> MakeForm()
        {
            ValidateAsyncDelegate<MeterReadings> validateSubscr =
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                async (state, value) =>
                {
                    var subscrCode = value as string;
                    var result = new ValidateResult();
                    var errorMessage = Validator.IsSubscrValid(subscrCode);
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        result.Feedback = errorMessage;
                        result.IsValid = false;
                        return result;
                    }
                    
                    var user = ApiHelper.GetUser(subscrCode);
                    state.User = await user;
                    if (state.User == null)
                    {
                        result.Feedback = "Извините, но мне не удалось идентифицировать Вас. Давайте попробуем снова.";
                        result.IsValid = false;
                        return result;
                    }

                    if (!string.IsNullOrEmpty(state.User.Message))
                    {
                        result.Feedback = state.User.Message;
                        result.IsValid = false;
                        return result;
                    }

                    result.Feedback = null;
                    result.IsValid = true;
                    result.Value = subscrCode;
                    return result;
                };
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            ValidateAsyncDelegate<MeterReadings> validateReading =
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                async (state, value) =>
                {
                    var reading = value as string;
                    var meter = state.User.Meters.FirstOrDefault(x => x.WebID.ToString() == state.MeterId);
                    var result = new ValidateResult();
                    var errorMessage = Validator.IsMeterValid(reading, meter);
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        result.Feedback = errorMessage;
                        result.IsValid = false;
                    }
                    else
                    {
                        result.Feedback = null;
                        result.IsValid = true;
                        result.Value = reading;
                    }
                    return result;
                };
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            OnCompletionAsyncDelegate<MeterReadings> processOrder = async (context, state) =>
            {
                await context.PostAsync("Спасибо! Принимаю показания...");
            };

            
            return new FormBuilder<MeterReadings>()
                .Message("Добро пожаловать в сервис приема показаний!")
                .Field("SubscrCode", null, validateSubscr)
                .Message(ShowHello)
                .Field(new FieldReflector<MeterReadings>("MeterId")
                    .SetType(null)
                    .SetDefine((state, field) =>
                    {
                        if (state.User != null)
                        {
                            var meters = state.User.Meters;
                            foreach (var meter in meters)
                                field
                                    .AddDescription(meter.WebID.ToString(), meter.ToString())
                                    .AddTerms(meter.WebID.ToString(), meter.WebID.ToString(), meter.ToString());
                        }
                        return Task.FromResult(true);
                    }))
                .Message(ShowPreviousReading)
                .Field("Reading", null, validateReading)
                //TODO: https://github.com/Microsoft/BotBuilder/issues/1324 - убрать термы с полей
                .Confirm("Вы хотите ввести показание {Reading} по прибору учета {MeterId}? {||}", null, new[] { "MeterId", "Reading" })
                
                .OnCompletion(processOrder)
                .Build();

        }

        private static Task<PromptAttribute> ShowHello(MeterReadings reading)
        {
            return Task.Run(() => new PromptAttribute(string.Format("Здравствуйте, {0}", reading.User.SubscrId)));
        }

        private static Task<PromptAttribute> ShowPreviousReading(MeterReadings reading)
        {
            return Task.Run(() =>
            {
                var meter = reading.User.Meters.FirstOrDefault(x => x.WebID.ToString() == reading.MeterId.ToString());
                if (meter == null)
                    return new PromptAttribute("Предыдущее показание не найдено");

                return new PromptAttribute("Последнее показание: " + string.Format(string.Format("{{0:{0}}}", "N" + meter.Unit), meter.BackMeterDec) + " - " + string.Format("{0:d}", meter.LastDate));
            });
        }
    }
}