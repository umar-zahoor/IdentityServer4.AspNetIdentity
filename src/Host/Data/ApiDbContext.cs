using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Host.Models;
using Microsoft.AspNetCore.Identity;

namespace Host.Data
{
    public class ApiDbContext : IdentityDbContext<ApiUser, ApiRole, Guid>
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            #region Schema Customizations

            builder.Entity<ApiUser>(typeBuilder =>
            {
                typeBuilder.ToTable("ApiUser");
                typeBuilder.HasKey(x => x.Id);
            });

            builder.Entity<ApiRole>(typeBuilder =>
            {
                typeBuilder.ToTable("ApiRole");
                typeBuilder.HasKey(x => x.Id);
            });

            builder.Entity<IdentityUserRole<Guid>>(typeBuilder =>
            {
                typeBuilder.ToTable("UserRoles");
                typeBuilder.HasKey(x => new {x.RoleId, x.UserId});
            });
            builder.Entity<IdentityUserLogin<Guid>>(typeBuilder =>
            {
                typeBuilder.ToTable("UserLogins");
                typeBuilder.HasKey(x => new {x.ProviderKey, x.LoginProvider});
            });
            builder.Entity<IdentityRoleClaim<Guid>>(typeBuilder =>
            {
                typeBuilder.ToTable("RoleClaims");
                typeBuilder.HasKey(x => x.Id);
            });
            builder.Entity<IdentityUserClaim<Guid>>(typeBuilder =>
            {
                typeBuilder.ToTable("UserClaims");
                typeBuilder.HasKey(x => x.Id);
            });
            builder.Entity<IdentityUserToken<Guid>>(typeBuilder =>
            {
                typeBuilder.ToTable("UserTokens");
                typeBuilder.HasKey(x => x.UserId);
            });

            #endregion
        }
    }
}