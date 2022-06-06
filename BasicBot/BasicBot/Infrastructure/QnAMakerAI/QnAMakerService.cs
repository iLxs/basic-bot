using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Extensions.Configuration;

namespace BasicBot.Infrastructure.QnAMakerAI
{
    public class QnAMakerService : IQnAMakerService
    {
        public QnAMaker _qnaMakerResult { get; set; }

        public QnAMakerService(IConfiguration configuration)
        {
            _qnaMakerResult = new QnAMaker(
                new QnAMakerEndpoint()
                {
                    KnowledgeBaseId = configuration["QnAMakerKnowledgeBaseId"],
                    EndpointKey = configuration["QnAMakerKey"],
                    Host = configuration["QnAMakerHost"],
                }
            );
        }
    }
}
