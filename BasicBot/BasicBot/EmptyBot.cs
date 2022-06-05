// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using BasicBot.Common.Models;
using BasicBot.Persistence;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BasicBot
{
    public class EmptyBot<T> : ActivityHandler where T : Dialog
    {
        private readonly BotState _userState;
        private readonly BotState _conversationState;
        private readonly Dialog _dialog;
        private readonly IDatabaseService _databaseService;

        public EmptyBot(UserState userState, ConversationState conversationState, T dialog, IDatabaseService databaseService)
        {
            _userState = userState;
            _conversationState = conversationState;
            _dialog = dialog;
            _databaseService = databaseService;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello world!"), cancellationToken);
                }
            }
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await SaveUserAsync(turnContext);
            await _dialog.RunAsync(
                    turnContext,
                    _conversationState.CreateProperty<DialogState>(nameof(DialogState)),
                    cancellationToken
                );
        }

        private async Task SaveUserAsync(ITurnContext<IMessageActivity> turnContext)
        {
            var userModel = new UserModel();
            userModel.id = turnContext.Activity.From.Id;
            userModel.usernameChannel = turnContext.Activity.From.Name;
            userModel.channel = turnContext.Activity.ChannelId;
            userModel.registerDate = DateTime.Now.Date;

            var user = await _databaseService.Users.FirstOrDefaultAsync(u => u.id == userModel.id);

            if (user == null)
            {
                await _databaseService.Users.AddAsync(userModel);
                await _databaseService.SaveAsync();
            }
        }
    }
}
