using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.DomainModels
{
    [Table("AspNetUsers")]
    public class Customer : IdentityUser {}

    public class EcomContext: IdentityDbContext<Customer>
    {
        public EcomContext() { }
        public EcomContext(DbContextOptions<EcomContext> options):base(options) { }

        public DbSet<Customer> Customers { get; set; }

        public static string ConnectionString = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=orcl)));User ID=system;Password=123123;";

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseOracle(EcomContext.ConnectionString);
            base.OnConfiguring(builder);
        }
    }
}
