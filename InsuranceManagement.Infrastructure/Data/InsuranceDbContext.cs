using InsuranceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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


            modelBuilder.Entity<Operator>()
            .HasMany(o => o.Categories)
            .WithMany(c => c.Operators)
            .UsingEntity(j => j.ToTable("OperatorOperatorCategories"));
        }
        
    }
}
