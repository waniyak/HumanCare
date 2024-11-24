using Microsoft.EntityFrameworkCore;
using HumanCare.Models;
using System.Data;
using HumanCare.Models.ViewModels;

namespace HumanCare.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Donation> Donations { get; set; }
        public DbSet<DonationCategory> DonationCategories { get; set; }
        public DbSet<Ngo> Ngos { get; set; }

        public DbSet<AboutUs> Aboutus { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<Partner> Partners { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define and seed roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" }
            );

            // Optionally, seed a default user and assign roles if needed
            modelBuilder.Entity<Users>().HasData(
                new Users
                {
                    Id = 1,
                    Username = "Admin",
                    Email = "admin@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123")
                }
            );

            // Seed user-role assignments if necessary
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    UserId = 1,
                    RoleId = 1 // Assign the Admin role to the default user
                }
            );

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);



            modelBuilder.Entity<DonationCategory>().HasData(
                new DonationCategory { Id = 1, Name = "Animal Shelter" },
                new DonationCategory { Id = 2, Name = "Blood Bank" },
                new DonationCategory { Id = 3, Name = "Children" },
                new DonationCategory { Id = 4, Name = "Education" },
                new DonationCategory { Id = 5, Name = "Food" },
                new DonationCategory { Id = 6, Name = "Hospital" },
                new DonationCategory { Id = 7, Name = "Morgue" },
                new DonationCategory { Id = 8, Name = "Orphanage Home" },
                new DonationCategory { Id = 9, Name = "Old Age Home" },
                new DonationCategory { Id = 10, Name = "Zakat" }
            );

            modelBuilder.Entity<Ngo>().HasData(
                new Ngo { Id = 1, Name = "Edhi Foundation" },
                new Ngo { Id = 2, Name = "Saylani Welfare International Trust" },
                new Ngo { Id = 3, Name = "Chhipa Welfare Association" },
                new Ngo { Id = 4, Name = "Care Foundation" },
                new Ngo { Id = 5, Name = "Human Rights Commission of Pakistan (HRCP)" },
                new Ngo { Id = 6, Name = "The Citizens Foundation (TCF)" },
                new Ngo { Id = 7, Name = "Pakistan Red Crescent Society" },
                new Ngo { Id = 8, Name = "Hunar Foundation" },
                new Ngo { Id = 9, Name = "Chhipa Welfare Association" }
            );
            // Seed data for AboutUs (if necessary)
            modelBuilder.Entity<AboutUs>().HasData(
                new AboutUs { Id = 1, Content = "This is the initial About Us content. Update this content as needed." }
            );
            // Configure entity mappings here if needed
            modelBuilder.Entity<Donation>().ToTable("Donations");
            modelBuilder.Entity<DonationCategory>().ToTable("DonationCategories");
            modelBuilder.Entity<Ngo>().ToTable("Ngos");
        }



        public DbSet<HumanCare.Models.ViewModels.PartnerViewModel> PartnerViewModel { get; set; } = default!;


    }
}