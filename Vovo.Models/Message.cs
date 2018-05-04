using Newtonsoft.Json;

namespace Vovo.Models
{
    public class Message : BaseModel
    {
        public int MessageID { get; set; }
        public int ChatID { get; set; }
        public string Content { get; set; }
        public string UserID { get; set; }
        public bool IsDelivered { get; set; }


        [JsonIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual Chat Chat { get; set; }
    }
}