using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vovo.Models
{
    public class UserFriend
    {
        [Key]
        public int Id { get; set; }
        public string UserUserID { get; set; }

        public string FriendUserID { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        public virtual User Friend { get; set; }
    }
}
