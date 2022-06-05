using System;

namespace BasicBot.Common.Models
{
    public class MedicalAppointmentModel
    {
        public string id { get; set; }
        public string idUser { get; set; }
        public DateTime date { get; set; }
        public int time { get; set; }
    }
}
