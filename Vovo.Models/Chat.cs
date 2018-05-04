using Newtonsoft.Json;
using System.Collections.Generic;

namespace Vovo.Models
{
    public class Chat : BaseModel
    {
        public int ChatID { get; set; }
        public string SenderUserID { get; set; }
        public string ReceiverUserID { get; set; }
        public string LastMessage { get; set; }

        //[JsonIgnore]
        public virtual User Sender { get; set; }

        //[JsonIgnore]
        public virtual User Receiver { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}