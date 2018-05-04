using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

public class VovoAPIContext : DbContext
{
    // You can add custom code to this file. Changes will not be overwritten.
    // 
    // If you want Entity Framework to drop and regenerate your database
    // automatically whenever you change your model schema, please use data migrations.
    // For more information refer to the documentation:
    // http://msdn.microsoft.com/en-us/data/jj591621.aspx

    public VovoAPIContext() : base("VovoAPIContext")
    {
    }

    public System.Data.Entity.DbSet<Vovo.Models.Chat> Chats { get; set; }

    public System.Data.Entity.DbSet<Vovo.Models.User> Users { get; set; }

    public System.Data.Entity.DbSet<Vovo.Models.Country> Countries { get; set; }

    public System.Data.Entity.DbSet<Vovo.Models.UserImage> UserImages { get; set; }

    public System.Data.Entity.DbSet<Vovo.Models.UserLocation> Locations { get; set; }

    public System.Data.Entity.DbSet<Vovo.Models.UserFriend> Friends { get; set; }
    public System.Data.Entity.DbSet<Vovo.Models.Message> Messages { get; set; }
}
