// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EmptyBot v4.15.2

using BasicBot.Dialogs;
using BasicBot.Infrastructure.Luis;
using BasicBot.Infrastructure.QnAMakerAI;
using BasicBot.Infrastructure.SendGrid;
using BasicBot.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Azure.Blobs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BasicBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient().AddControllers().AddNewtonsoftJson();

            // Cosmos db configuration
            services.AddDbContext<DatabaseService>(options =>
            {
                options.UseCosmos(
                    Configuration["CosmosDbEndpoint"],
                    Configuration["CosmosDbAuthKey"],
                    Configuration["CosmosDbDatabaseId"]
                    );
            });

            services.AddScoped<IDatabaseService, DatabaseService>();

            // Blob storage db configuration
            var storage = new BlobsStorage(
                    Configuration.GetValue<string>("BlobConnectionString"),
                    Configuration.GetValue<string>("BlobContainerName")
                    );
            services.AddSingleton<IStorage>(storage);

            // Create the User state. (Used in this bot's Dialog implementation.)
            var userState = new UserState(storage);
            services.AddSingleton(userState);

            // Create the Conversation state. (Used by the Dialog system itself.)
            var conversationState = new ConversationState(storage);
            services.AddSingleton(conversationState);

            // Create the Bot Framework Authentication to be used with the Bot Adapter.
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

            // Create the Bot Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create Luis service
            services.AddSingleton<ILuisService, LuisService>();

            // Create QnA Maker service
            services.AddSingleton<IQnAMakerService, QnAMakerService> ();

            // Create SendGrid service
            services.AddSingleton<ISendGridService, SendGridService>();

            // Register dialog
            services.AddTransient<RootDialog>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, EmptyBot<RootDialog>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
