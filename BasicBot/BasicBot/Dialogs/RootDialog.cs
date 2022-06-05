using BasicBot.Common.Cards;
using BasicBot.Common.Constants;
using BasicBot.Dialogs.CreateAppointment;
using BasicBot.Dialogs.Qualification;
using BasicBot.Infrastructure.Luis;
using BasicBot.Persistence;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BasicBot.Dialogs
{
    public class RootDialog : ComponentDialog
    {
        private readonly ILuisService _luisService;
        private readonly IDatabaseService _databaseService;

        public RootDialog(ILuisService luisService, IDatabaseService databaseService)
        {
            _luisService = luisService;
            _databaseService = databaseService;

            // Create the steps of our waterfall dialog
            var waterfallSteps = new WaterfallStep[]
            {
                InitialProcess,
                FinalProcess
            };

            // Add the dialogs to use
            
            AddDialog(new QualificationDialog(_databaseService));
            AddDialog(new CreateAppointmentDialog(_databaseService));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            // Set out waterfall dialog as the initial dialog of this dialog
            InitialDialogId = nameof(WaterfallDialog);
        }

        #region Waterfall Steps

        private async Task<DialogTurnResult> InitialProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var luisResult = await _luisService._luisRecognizer.RecognizeAsync(stepContext.Context, cancellationToken);
            return await ManageIntentions(stepContext, luisResult, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        #endregion

        private async Task<DialogTurnResult> ManageIntentions(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var topIntent = luisResult.GetTopScoringIntent();
            switch (topIntent.intent)
            {
                case Constants.INTENT_SALUDAR:
                    await IntentSaludar(stepContext, luisResult, cancellationToken);
                    break;
                case Constants.INTENT_AGRADECER:
                    await IntentAgradecer(stepContext, luisResult, cancellationToken);
                    break;
                case Constants.INTENT_DESPEDIR:
                    await IntentDespedir(stepContext, luisResult, cancellationToken);
                    break;
                case Constants.INTENT_NONE:
                    await IntentNone(stepContext, luisResult, cancellationToken);
                    break;
                case Constants.INTENT_VER_OPCIONES:
                    await IntentVerOpciones(stepContext, luisResult, cancellationToken);
                    break;
                case Constants.INTENT_VER_CENTRO_CONTRACTO:
                    await IntentVerCentroContacto(stepContext, luisResult, cancellationToken);
                    break;
                case Constants.INTENT_CALIFICAR:
                    return await IntentCalificar(stepContext, luisResult, cancellationToken);
                case Constants.INTENT_CREAR_CITA:
                    return await IntentCrearCita(stepContext, luisResult, cancellationToken);
                default:
                    break;
            }
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        #region Luis Intents

        private async Task IntentSaludar(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var message = "Hola, que gusto verte";
            await stepContext.Context.SendActivityAsync(message, cancellationToken: cancellationToken);
        }

        private async Task IntentAgradecer(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var message = "No te preocupes, me gusta ayudar";
            await stepContext.Context.SendActivityAsync(message, cancellationToken: cancellationToken);
        }

        private async Task IntentDespedir(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var message = "Espero verte pronto";
            await stepContext.Context.SendActivityAsync(message, cancellationToken: cancellationToken);
        }

        private async Task IntentNone(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var message = "No pude entender lo que dijiste, intenta con otras palabras";
            await stepContext.Context.SendActivityAsync(message, cancellationToken: cancellationToken);
        }

        private async Task IntentVerOpciones(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var message = "Estas son mis opciones";
            await stepContext.Context.SendActivityAsync(message, cancellationToken: cancellationToken);
            await MainOptionsCard.Send(stepContext, cancellationToken);
        }

        private async Task IntentVerCentroContacto(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            string phoneDetail = $"Nuestro números de atención son los siguientes: {Environment.NewLine}" +
                $"📞 +51 987654321{Environment.NewLine} 📞 +51 987456123";
            string addressDetail = $"🏢 Estamos ubicados en {Environment.NewLine} Calle Chatbot 457, La Molina, Lima";

            await stepContext.Context.SendActivityAsync(addressDetail, cancellationToken: cancellationToken);
            await Task.Delay(2000);
            await stepContext.Context.SendActivityAsync(phoneDetail, cancellationToken: cancellationToken);
            var helpMessage = "¿En qué más te puedo ayudar?";
            await stepContext.Context.SendActivityAsync(helpMessage, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> IntentCalificar(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(QualificationDialog), cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> IntentCrearCita(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(CreateAppointmentDialog), cancellationToken: cancellationToken);
        }

        #endregion
    }
}
