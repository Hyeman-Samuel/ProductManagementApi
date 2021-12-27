using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductManagementApi.Identity;
using ProductManagementApi.Models;
using ProductManagementApi.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Persistence
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public PasswordHasher<ApplicationUser> Hasher { get; set; } = new PasswordHasher<ApplicationUser>();
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
        }     
        public DbSet<Product> Products { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedUser(builder);
        }

        private void SeedUser(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Name = Identity.Roles.Admin,
                NormalizedName = Identity.Roles.Admin,
                Id = Constants.SystemRoleId,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });


            builder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Email = Constants.SystemUserEmail,
                Id = Constants.SystemUserId,
                NormalizedEmail = Constants.SystemUserEmail.ToUpper(),
                NormalizedUserName = Constants.SystemUserEmail.ToUpper(),
                UserName = Constants.SystemUserEmail,
                PasswordHash = Hasher.HashPassword(null, Constants.SystemUserPassword),
                SecurityStamp = Guid.NewGuid().ToString()
            });

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = Constants.SystemRoleId,
                UserId = Constants.SystemUserId
            });
        }







    }
}
