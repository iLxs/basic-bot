using BasicBot.Common.Models;
using BasicBot.Persistence;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BasicBot.Dialogs.Qualification
{
    public class QualificationDialog : ComponentDialog
    {
        private readonly IDatabaseService _databaseService;

        public QualificationDialog(IDatabaseService databaseService)
        {
            _databaseService = databaseService;

            var waterfallSteps = new WaterfallStep[]
            {
                ShowButton,
                ValidateOption
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ShowButton(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                 nameof(TextPrompt),
                 new PromptOptions
                 {
                     Prompt = CreateButtonsQualification()
                 },
                 cancellationToken
             );
        }

        private async Task<DialogTurnResult> ValidateOption(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var selectedOption = stepContext.Context.Activity.Text;
            var message = $"Gracias por tu {selectedOption}";
            await stepContext.Context.SendActivityAsync(message, cancellationToken: cancellationToken);
            var helpMessage = "¿En qué más te puedo ayudar?";
            await stepContext.Context.SendActivityAsync(helpMessage, cancellationToken: cancellationToken);

            await SaveQualification(stepContext, selectedOption);
            return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);
        }

        private Activity CreateButtonsQualification()
        {
            var reply = MessageFactory.Text("Calificame con una de las siguientes opciones");
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "1⭐", Value = "1⭐", Type = ActionTypes.ImBack },
                    new CardAction() { Title = "2⭐", Value = "2⭐", Type = ActionTypes.ImBack },
                    new CardAction() { Title = "3⭐", Value = "3⭐", Type = ActionTypes.ImBack },
                    new CardAction() { Title = "4⭐", Value = "4⭐", Type = ActionTypes.ImBack },
                    new CardAction() { Title = "5⭐", Value = "5⭐", Type = ActionTypes.ImBack }
                }
            };
            return reply as Activity;
        }

        private async Task SaveQualification(WaterfallStepContext stepContext, string selectedOption)
        {
            var qualificationModel = new QualificationModel();
            qualificationModel.id = Guid.NewGuid().ToString();
            qualificationModel.idUser = stepContext.Context.Activity.From.Id;
            qualificationModel.qualification = selectedOption;
            qualificationModel.registerDate = DateTime.Now;

            await _databaseService.Qualifications.AddAsync(qualificationModel);
            await _databaseService.SaveAsync();
        }
    }
}
