namespace BasicBot.Common.Constants
{
    public class Constants
    {
        public const string INTENT_NONE = "None";
        public const string INTENT_SALUDAR = "Saludar";
        public const string INTENT_DESPEDIR = "Despedir";
        public const string INTENT_AGRADECER = "Agradecer";
        public const string INTENT_CALIFICAR = "Calificar";
        public const string INTENT_VER_OPCIONES = "VerOpciones";
        public const string INTENT_CREAR_CITA = "CrearCita";
        public const string INTENT_VER_CITA = "VerCita";
        public const string INTENT_VER_CENTRO_CONTRACTO = "VerCentroContacto";

        public const string MENU_01_IMAGE = "https://botcurso96.blob.core.windows.net/images/Menu-01.jpg";
        public const string MENU_02_IMAGE = "https://botcurso96.blob.core.windows.net/images/Menu-02.jpg";
        public const string MENU_03_IMAGE = "https://botcurso96.blob.core.windows.net/images/Menu-03.jpg";
        public const string MENU_04_IMAGE = "https://botcurso96.blob.core.windows.net/images/Menu-04.jpg";

        public const string WEB_SITE_URL = "https://docs.microsoft.com/en-us/azure/bot-service/index-bf-sdk?view=azure-bot-service-4.0";
        public const string FACEBOOK_URL = "https://www.facebook.com";
        public const string INSTAGRAM_URL = "https://www.instagram.com/";
        public const string TWITTER_URL = "https://twitter.com/";

        public const string BOT_EMAIL = "lxs-96@hotmail.com";
        public const string BOT_NAME = "Basic Bot";

        public const string SUBJECT_NUEVA_CITA = "Confirmación de cita";
        public const string HTML_BODY = "Hola {0},<br><br>" +
            "El correo es para confirmarte la creación de una cita con la siguiente información:<br>" +
            "Fecha: {1}<br>" +
            "Hora: {2}<br><br>" +
            "Que tengas un buen día,<br>" +
            "Basic Bot.";
    }
}
