using System;
using System.Collections.Generic;
using System.Text;
using CA2_Assignment.Models.CscModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CA2_Ultima.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<PremiumMembers> PremiumMembers { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
