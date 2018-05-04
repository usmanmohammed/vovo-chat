using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vovo.Models
{
    public class UserImage
    {
        [Key, ForeignKey("User")]
        public string UserID { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public virtual User User { get; set; }
    }
}