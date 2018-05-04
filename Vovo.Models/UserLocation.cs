using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vovo.Models
{
    public class UserLocation : BaseModel
    {
        [Key]
        public int LocationID { get; set; }
        public string UserID { get; set; }
        public string Coordinates { get; set; }
        public string Address { get; set; }
        public User User { get; set; }
    }
}
