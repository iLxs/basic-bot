using System.Collections.Generic;

namespace BasicBot.Common.Models
{
    public class EntityLuisModel
    {
        public List<DatetimeEntity> datetime { get; set; }
    }

    public class DatetimeEntity
    {
        public string type { get; set; }
        public List<string> timex { get; set; }
    }
}
