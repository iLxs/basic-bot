using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;

namespace BasicBot.Infrastructure.Luis
{
    public class LuisService : ILuisService
    {
        public LuisRecognizer _luisRecognizer { get; set; }

        public LuisService(IConfiguration configuration)
        {
            var luisIsConfigured = !string.IsNullOrEmpty(configuration["LuisAppId"])
                && !string.IsNullOrEmpty(configuration["LuisAPIKey"])
                && !string.IsNullOrEmpty(configuration["LuisAPIHostName"]);

            if (luisIsConfigured)
            {
                var luisApplicaton = new LuisApplication(
                    configuration["LuisAppId"],
                    configuration["LuisAPIKey"],
                    configuration["LuisAPIHostName"]
                );

                var recognizerOptions = new LuisRecognizerOptionsV3(luisApplicaton)
                {
                    PredictionOptions = new Microsoft.Bot.Builder.AI.LuisV3.LuisPredictionOptions
                    {
                        IncludeInstanceData = true,
                    }
                };
                
                _luisRecognizer = new LuisRecognizer(recognizerOptions);
            }
        }
    }
}
