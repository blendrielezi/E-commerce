// ProjektSezon2/Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjektSezon2.Models;

namespace ProjektSezon2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Domain entities (nullable to satisfy compiler)
        public DbSet<Session>? Sessions { get; set; }
        public DbSet<CookieConsent>? CookieConsents { get; set; }
        public DbSet<Service>? Services { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public DbSet<OrderItem>? OrderItems { get; set; }
        public DbSet<PaymentTransaction>? PaymentTransactions { get; set; }
        public DbSet<FilterSetting>? FilterSettings { get; set; }
        public DbSet<Category>? Categories { get; set; }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<OrderItem>()
                   .HasOne(oi => oi.Order)
                   .WithMany(o => o.Items)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItem>()
                   .HasOne(oi => oi.Service)
                   .WithMany()
                   .HasForeignKey(oi => oi.ServiceId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PaymentTransaction>()
                   .HasOne(pt => pt.Order)
                   .WithMany(o => o.PaymentTransactions)   
                   .HasForeignKey(pt => pt.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Session>()
                   .HasOne(s => s.ApplicationUser)
                   .WithMany(u => u.Sessions)
                   .HasForeignKey(s => s.ApplicationUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CookieConsent>()
                   .HasOne(cc => cc.ApplicationUser)
                   .WithMany(u => u.CookieConsents)
                   .HasForeignKey(cc => cc.ApplicationUserId)
                   .OnDelete(DeleteBehavior.Cascade);

          


            builder.Entity<Service>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Services)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
