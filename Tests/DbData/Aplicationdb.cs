using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Tests.Models;

namespace MvcApp.Models
{
    public class Aplicationdb : DbContext
    {

        public DbSet<TestModel> Tests { get; set; }

        public DbSet<QuestionModel> Questions { get; set; }

        public DbSet<AnswerModel> Answers { get; set; }

        public DbSet<TestResultModel> TestResults { get; set; }

        public DbSet<SubjectViewModel> Subjects { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<TestModel>()
                .HasOne(t => t.SubjectViewModel)
                .WithMany(s => s.Tests)
                .HasForeignKey(t => t.SubjectViewModelId);

            modelBuilder.Entity<QuestionModel>()
                .HasOne(q => q.test)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.TestId);

            modelBuilder.Entity<AnswerModel>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId);
            modelBuilder.Entity<TestResultModel>()
                .Property(e => e.SelectedAnswers)
                .HasConversion(new SelectedAnswersConverter());

            modelBuilder.Entity<TestResultModel>()
                .Property(e => e.TextAnswers)
                .HasConversion(new TextAnswersConverter());
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId);

            SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);

        }
        private void SeedData(ModelBuilder modelBuilder)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes("1234567890");
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { Id = 1, UserId = 1, Role = RoleAdmin.SuperAdmin.ToString() }
            );


            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Login = "superadmin",
                    Password = sb.ToString(), 

                }
            );
        }
        public Aplicationdb(DbContextOptions<Aplicationdb> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }
        public QuestionModel GetQuestionById(int testId, int questionId)
        {
            return Questions.FirstOrDefault(q => q.TestId == testId && q.Id == questionId);
        }
    }
}