using System;

namespace BasicBot.Common.Models
{
    public class UserModel
    {
        public string id { get; set; }
        public string usernameChannel { get; set; }
        public string channel { get; set; }
        public DateTime registerDate { get; set; }
        public string phone { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
    }
}
