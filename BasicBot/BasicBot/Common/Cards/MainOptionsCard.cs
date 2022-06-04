using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BasicBot.Common.Cards
{
    public class MainOptionsCard
    {
        public static async Task Send(DialogContext dialogContext, CancellationToken cancellationToken)
        {
            await dialogContext.Context.SendActivityAsync(activity: CreateCarousel(), cancellationToken);
        }

        private static Activity CreateCarousel()
        {
            var cardCitasMedicas = new HeroCard()
            {
                Title = "Citas médicas",
                Subtitle = "Opciones",
                Images = new List<CardImage> { new CardImage(Constants.Constants.MENU_01_IMAGE) },
                Buttons = new List<CardAction>()
                {
                    new CardAction(){ Title = "Crear cita médica", Value = "Crear cita médica", Type = ActionTypes.ImBack },
                    new CardAction(){ Title = "Ver mi cita", Value = "Ver mi cita", Type = ActionTypes.ImBack }
                }
            };

            var cardInformacionContacto = new HeroCard()
            {
                Title = "Información de contacto",
                Subtitle = "Opciones",
                Images = new List<CardImage> { new CardImage(Constants.Constants.MENU_02_IMAGE) },
                Buttons = new List<CardAction>()
                {
                    new CardAction(){ Title = "Centro de contacto", Value = "Centro de contacto", Type = ActionTypes.ImBack },
                    new CardAction(){ Title = "Sitio web", Value = Constants.Constants.WEB_SITE_URL, Type = ActionTypes.OpenUrl }
                }
            };

            var cardRedesSociales = new HeroCard()
            {
                Title = "Síguenos en las redes",
                Subtitle = "Opciones",
                Images = new List<CardImage> { new CardImage(Constants.Constants.MENU_03_IMAGE) },
                Buttons = new List<CardAction>()
                {
                    new CardAction(){ Title = "Facebook", Value = Constants.Constants.FACEBOOK_URL, Type = ActionTypes.OpenUrl },
                    new CardAction(){ Title = "Instagram", Value = Constants.Constants.INSTAGRAM_URL, Type = ActionTypes.OpenUrl },
                    new CardAction(){ Title = "Twitter", Value = Constants.Constants.TWITTER_URL, Type = ActionTypes.OpenUrl }
                }
            };

            var cardCalificar = new HeroCard()
            {
                Title = "Calificación",
                Subtitle = "Opciones",
                Images = new List<CardImage> { new CardImage(Constants.Constants.MENU_04_IMAGE) },
                Buttons = new List<CardAction>()
                {
                    new CardAction(){ Title = "Calificar bot", Value = "Calificar bot", Type = ActionTypes.ImBack },
                }
            };

            var optionsAttachments = new List<Attachment>()
            {
                cardCitasMedicas.ToAttachment(),
                cardInformacionContacto.ToAttachment(),
                cardRedesSociales.ToAttachment(),
                cardCalificar.ToAttachment()
            };

            var reply = MessageFactory.Attachment(optionsAttachments);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            return reply as Activity;
        }
    }
}
