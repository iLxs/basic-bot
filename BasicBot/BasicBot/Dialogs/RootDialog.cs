using BasicBot.Common.Cards;
using BasicBot.Common.Constants;
using BasicBot.Infrastructure.Luis;
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

        public RootDialog(ILuisService luisService)
        {
            _luisService = luisService;

            // Create the steps of our waterfall dialog
            var waterfallSteps = new WaterfallStep[]
            {
                InitialProcess,
                FinalProcess
            };

            // Create the waterfall dialog
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

        #endregion
    }
}
