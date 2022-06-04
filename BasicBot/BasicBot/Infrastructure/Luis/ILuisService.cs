using Microsoft.Bot.Builder.AI.Luis;

namespace BasicBot.Infrastructure.Luis
{
    public interface ILuisService
    {
        LuisRecognizer _luisRecognizer { get; }
    }
}
