using Microsoft.Bot.Builder.AI.QnA;

namespace BasicBot.Infrastructure.QnAMakerAI
{
    public interface IQnAMakerService
    {
        QnAMaker _qnaMakerResult { get; set; }
    }
}
