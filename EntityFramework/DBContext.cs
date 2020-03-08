using Microsoft.EntityFrameworkCore;
using Repair.EntityFramework.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repair.EntityFramework
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {

        }


        public DbSet<User> users { get; set; }

        public DbSet<Community> Communities { get; set; }

        public DbSet<RepairMan> RepairMen { get; set; }

        public DbSet<RepairList> RepairLists { get; set; }

        public DbSet<RepairListInfo> RepairListInfos { get; set; }
        
        public DbSet<AdminRole> AdminRoles { get; set; }
        
        public DbSet<RepairManRole> RepairManRoles { get; set; }
    }
}
