using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TB
{
    public class AppContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=TelegramBotDB.db");

        public DbSet<PhoneNumber> PhoneNumber { get; set; }
    }
}
