using BasicBot.Common.Constants;
using BasicBot.Common.Models;
using BasicBot.Infrastructure.Luis;
using BasicBot.Infrastructure.SendGrid;
using BasicBot.Persistence;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BasicBot.Dialogs.CreateAppointment
{
    public class CreateAppointmentDialog : ComponentDialog
    {
        private readonly IDatabaseService _databaseService;
        private readonly ISendGridService _sendGridService;

        public static UserModel newUserModel = new UserModel();
        public static MedicalAppointmentModel newMedicalAppointmentModel = new MedicalAppointmentModel();

        private readonly IStatePropertyAccessor<BotStateModel> _userState;

        private static string userText;
        private readonly ILuisService _luisService;

        public CreateAppointmentDialog(IDatabaseService databaseService, UserState userState, ISendGridService sendGridService, ILuisService luisService)
        {
            _databaseService = databaseService;
            _sendGridService = sendGridService;
            _luisService = luisService;
            _userState = userState.CreateProperty<BotStateModel>(nameof(BotStateModel));

            var waterfallSteps = new WaterfallStep[]
            {
                SetPhone,
                SetFullName,
                SetEmail,
                SetDate,
                SetTime,
                Confirmation,
                FinalProcess
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> SetPhone(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            userText = stepContext.Context.Activity.Text;

            if (await UserHasMedicalData(stepContext))
            {
                return await stepContext.NextAsync(cancellationToken: cancellationToken);
            }

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions()
                {
                    Prompt = MessageFactory.Text("Por favor, ingresa tu número de teléfono:")
                },
                cancellationToken: cancellationToken
            );
        }

        private async Task<DialogTurnResult> SetFullName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (await UserHasMedicalData(stepContext))
            {
                return await stepContext.NextAsync(cancellationToken: cancellationToken);
            }

            var userPhone = stepContext.Context.Activity.Text;
            newUserModel.phone = userPhone;

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions()
                {
                    Prompt = MessageFactory.Text("Ahora ingresa tu nombre completo")
                },
                cancellationToken: cancellationToken
            );
        }

        private async Task<DialogTurnResult> SetEmail(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (await UserHasMedicalData(stepContext))
            {
                return await stepContext.NextAsync(cancellationToken: cancellationToken);
            }

            var fullName = stepContext.Context.Activity.Text;
            newUserModel.fullName = fullName;

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions()
                {
                    Prompt = MessageFactory.Text("Ahora ingresa tu correo")
                },
                cancellationToken: cancellationToken
            );
        }

        private async Task<DialogTurnResult> SetDate(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!await UserHasMedicalData(stepContext))
            {
                var email = stepContext.Context.Activity.Text;
                newUserModel.email = email;
            }

            var newStepContext = stepContext;
            stepContext.Context.Activity.Text = userText;
            var luisResult = await _luisService._luisRecognizer.RecognizeAsync(newStepContext.Context, cancellationToken: cancellationToken);

            var entity = luisResult.Entities.ToObject<EntityLuisModel>();

            if (entity.datetime != null)
            {
                var date = entity.datetime.First().timex.First().Replace("XXXX", DateTime.Now.Year.ToString());
                if (date.Length > 10)
                {
                    date = date.Remove(10);
                }

                newMedicalAppointmentModel.date = DateTime.Parse(date);

                return await stepContext.NextAsync(cancellationToken: cancellationToken);
            }

            string message = $"Ahora necesito la fecha de la cita médica con el siguiente formato: "+
                $"{Environment.NewLine}dd/mm/yyyy";
            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions()
                {
                    Prompt = MessageFactory.Text(message)
                },
                cancellationToken: cancellationToken
            );
        }

        private async Task<DialogTurnResult> SetTime(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (newMedicalAppointmentModel.date == DateTime.MinValue)
            {
                var medicalDate = stepContext.Context.Activity.Text;
                newMedicalAppointmentModel.date = Convert.ToDateTime(medicalDate);
            }

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions()
                {
                    Prompt = CreateButtonsTime()
                },
                cancellationToken: cancellationToken
            );
        }

        private async Task<DialogTurnResult> Confirmation(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var medicalTime = stepContext.Context.Activity.Text;
            newMedicalAppointmentModel.time = int.Parse(medicalTime);

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions()
                {
                    Prompt = CreateButtonsConfirmation()
                },
                cancellationToken: cancellationToken
            );
        }

        private async Task<DialogTurnResult> FinalProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var confirmationOption = stepContext.Context.Activity.Text;
            if (confirmationOption.ToLowerInvariant().Equals("si"))
            {
                string userId = stepContext.Context.Activity.From.Id;
                var userModel = await _databaseService.Users.FirstOrDefaultAsync(x => x.id == userId);

                var userStateModel = await _userState.GetAsync(stepContext.Context, () => new BotStateModel());

                //Update user model if has no medical data
                if (!userStateModel.hasMedicalData)
                {
                    userModel.phone = newUserModel.phone;
                    userModel.fullName = newUserModel.fullName;
                    userModel.email = newUserModel.email;

                    _databaseService.Users.Update(userModel);
                    await _databaseService.SaveAsync();
                }

                //Create medical appointment model
                newMedicalAppointmentModel.id = Guid.NewGuid().ToString();
                newMedicalAppointmentModel.idUser = userId;
                await _databaseService.MedicalAppointments.AddAsync(newMedicalAppointmentModel);
                await _databaseService.SaveAsync();

                await stepContext.Context.SendActivityAsync("Tu cita se guardó con éxito", cancellationToken: cancellationToken);

                //Set user hasMedicalData to true
                userStateModel.hasMedicalData = true;

                //Show appointment summary
                string summary = $"Para: {userModel.fullName}" +
                    $"{Environment.NewLine}📞 Teléfono: {userModel.phone}" +
                    $"{Environment.NewLine}✉ Email: {userModel.email}" +
                    $"{Environment.NewLine}📅 Fecha: {newMedicalAppointmentModel.date.ToShortDateString()}" +
                    $"{Environment.NewLine}🕛 Hora: {newMedicalAppointmentModel.time}";
                await stepContext.Context.SendActivityAsync(summary, cancellationToken: cancellationToken);

                await SendEmail(userModel, newMedicalAppointmentModel);

                var helpMessage = "¿En qué más te puedo ayudar?";
                await stepContext.Context.SendActivityAsync(helpMessage, cancellationToken: cancellationToken);

                newMedicalAppointmentModel = new MedicalAppointmentModel();
            }
            else
            {
                await stepContext.Context.SendActivityAsync("No hay problema, será para la próxima", cancellationToken: cancellationToken);
            }
            return await stepContext.ContinueDialogAsync(cancellationToken);
        }

        private Activity CreateButtonsTime()
        {
            var reply = MessageFactory.Text("Ahora selecciona la hora");
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "9", Value = "9", Type = ActionTypes.ImBack },
                    new CardAction() { Title = "10", Value = "10", Type = ActionTypes.ImBack },
                    new CardAction() { Title = "11", Value = "11", Type = ActionTypes.ImBack },
                    new CardAction() { Title = "15", Value = "15", Type = ActionTypes.ImBack },
                    new CardAction() { Title = "16", Value = "16", Type = ActionTypes.ImBack },
                    new CardAction() { Title = "17", Value = "17", Type = ActionTypes.ImBack },
                    new CardAction() { Title = "18", Value = "18", Type = ActionTypes.ImBack }
                }
            };
            return reply as Activity;
        }

        private Activity CreateButtonsConfirmation()
        {
            var reply = MessageFactory.Text("¿Confirmas la creación de esta cita médica?");
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "Si", Value = "Si", Type = ActionTypes.ImBack },
                    new CardAction() { Title = "No", Value = "No", Type = ActionTypes.ImBack }
                }
            };
            return reply as Activity;
        }

        private async Task<bool> UserHasMedicalData(WaterfallStepContext stepContext)
        {
            var userStateModel = await _userState.GetAsync(stepContext.Context, () => new BotStateModel());

            return userStateModel.hasMedicalData;
        }

        private async Task SendEmail(UserModel userModel, MedicalAppointmentModel medicalAppointmentModel)
        {
            var htmlBody = string.Format(Constants.HTML_BODY, userModel.fullName, medicalAppointmentModel.date.ToShortDateString(), medicalAppointmentModel.time);
            await _sendGridService.Execute(userModel.email, userModel.fullName, Constants.SUBJECT_NUEVA_CITA, null, htmlBody);
        }

    }
}
