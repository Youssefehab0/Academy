using Academy.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Academy.Infrastructure.Data
{
    public class AcademyDbContext : DbContext
    {
        public AcademyDbContext(DbContextOptions<AcademyDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Student> Students { get; set; }
        public DbSet<Admin> Admins { get; set; } 
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraint on Student Email
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            // Unique constraint on Admin Email
            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Email)
                .IsUnique();

            // Relations: Instructor -> Courses (1-M)
            modelBuilder.Entity<Instructor>()
                .HasMany(i => i.Courses)
                .WithOne(c => c.Instructor)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relations: Student -> Bookings (1-M)
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Bookings)
                .WithOne(b => b.Student)
                .HasForeignKey(b => b.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relations: Course -> Bookings (1-M)
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Bookings)
                .WithOne(b => b.Course)
                .HasForeignKey(b => b.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking -> Payment (1-1 optional)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Payment)
                .WithOne(p => p.Booking)
                .HasForeignKey<Payment>(p => p.BookingId);

            // Store enums as strings for readability in DB
            modelBuilder.Entity<Booking>()
                .Property(b => b.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Payment>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Payment>()
                .Property(p => p.Method)
                .HasConversion<string>();
            var adminHasher = new PasswordHasher<Admin>();

            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    FullName = "Youssef Ehab",
                    Email = "youssef.0523029@gmail.com",
                    PasswordHash = adminHasher.HashPassword(null!, "Y1042007")
                },
                new Admin
                {
                    Id = 2,
                    FullName = "Hatem Medhat",
                    Email = "hatemmedhat247@gmail.com",
                    PasswordHash = adminHasher.HashPassword(null!, "Hatem_247")
                }
            );
        }
    }
}