using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vovo.Models
{
    public class User : BaseModel
    {
        public string UserID { get; set; }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsActive { get; set; }
        public string PasswordHash { get; set; }
        public int? CountryID { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsLocationEnabled { get; set; }
        public string Location { get; set; }

        [JsonIgnore]

        public virtual Country Country { get; set; }

        [JsonIgnore]
        public virtual UserImage UserImage { get; set; }
    }
}
