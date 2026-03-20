using InsuranceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InsuranceManagement.Infrastructure.Data
{
    public class InsuranceDbContext : DbContext
    {
        public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<OperatorCategory> OperatorCategories { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Finance> Finances { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<MotorInsuranceApplication> MotorInsuranceApplications { get; set; }
        public DbSet<LifeInsuranceApplication> LifeInsuranceApplications { get; set; }
        public DbSet<ClaimR> Claims { get; set; }
        public DbSet<LifeInsuranceBeneficiary> Beneficiaries { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Announcement> Announcements { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Operator)
                .WithOne(m => m.User)
                .HasForeignKey<Operator>(m => m.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Client)
                .WithOne(m => m.User)
                .HasForeignKey<Client>(m => m.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Finance)
                .WithOne(f => f.User)
                .HasForeignKey<Finance>(f => f.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Manager)
                .WithOne(m => m.User)
                .HasForeignKey<Manager>(m => m.UserId);

            modelBuilder.Entity<Operator>()
            .HasMany(o => o.Categories)
            .WithMany(c => c.Operators)
            .UsingEntity(j => j.ToTable("OperatorOperatorCategories"));

            modelBuilder.Entity<OperatorCategory>()
                .HasOne(c => c.Parent)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClaimR>()
    .HasOne(c => c.MotorInsuranceApplication)
    .WithMany()
    .HasForeignKey(c => c.MotorInsuranceApplicationId)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClaimR>()
                .HasOne(c => c.LifeInsuranceApplication)
                .WithMany()
                .HasForeignKey(c => c.LifeInsuranceApplicationId)
                .OnDelete(DeleteBehavior.Restrict);


        }

    }
}
