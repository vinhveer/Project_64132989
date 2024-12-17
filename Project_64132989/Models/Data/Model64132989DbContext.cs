using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Project_64132989.Models.Data
{
    public partial class Model64132989DbContext : DbContext
    {
        public Model64132989DbContext()
            : base("name=Model64132989DbContext")
        {
        }

        public virtual DbSet<AdministrativeClass> AdministrativeClasses { get; set; }
        public virtual DbSet<CourseOffering> CourseOfferings { get; set; }
        public virtual DbSet<Cours> Courses { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Semester> Semesters { get; set; }
        public virtual DbSet<StudentCourseRegistration> StudentCourseRegistrations { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdministrativeClass>()
                .Property(e => e.class_id)
                .IsUnicode(false);

            modelBuilder.Entity<AdministrativeClass>()
                .Property(e => e.advisor_user_id)
                .IsUnicode(false);

            modelBuilder.Entity<AdministrativeClass>()
                .HasMany(e => e.Students)
                .WithRequired(e => e.AdministrativeClass)
                .HasForeignKey(e => e.administrative_class_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseOffering>()
                .Property(e => e.teacher_user_id)
                .IsUnicode(false);

            modelBuilder.Entity<CourseOffering>()
                .Property(e => e.section)
                .IsUnicode(false);

            modelBuilder.Entity<CourseOffering>()
                .HasMany(e => e.StudentCourseRegistrations)
                .WithRequired(e => e.CourseOffering)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Cours>()
                .Property(e => e.course_code)
                .IsUnicode(false);

            modelBuilder.Entity<Cours>()
                .HasMany(e => e.CourseOfferings)
                .WithRequired(e => e.Cours)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Cours>()
                .HasMany(e => e.Courses1)
                .WithOptional(e => e.Cours1)
                .HasForeignKey(e => e.prerequisite_course_id);

            modelBuilder.Entity<Grade>()
                .Property(e => e.midterm_score)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Grade>()
                .Property(e => e.final_score)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Grade>()
                .Property(e => e.total_score)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Grade>()
                .Property(e => e.grade_letter)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Grade>()
                .Property(e => e.grade_point)
                .HasPrecision(3, 2);

            modelBuilder.Entity<Grade>()
                .Property(e => e.partial_score)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Profile>()
                .Property(e => e.user_id)
                .IsUnicode(false);

            modelBuilder.Entity<Profile>()
                .Property(e => e.phone_number)
                .IsUnicode(false);

            modelBuilder.Entity<Semester>()
                .HasMany(e => e.CourseOfferings)
                .WithRequired(e => e.Semester)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<StudentCourseRegistration>()
                .Property(e => e.student_user_id)
                .IsUnicode(false);

            modelBuilder.Entity<StudentCourseRegistration>()
                .HasOptional(e => e.Grade)
                .WithRequired(e => e.StudentCourseRegistration);

            modelBuilder.Entity<Student>()
                .Property(e => e.user_id)
                .IsUnicode(false);

            modelBuilder.Entity<Student>()
                .Property(e => e.administrative_class_id)
                .IsUnicode(false);

            modelBuilder.Entity<Student>()
                .HasMany(e => e.StudentCourseRegistrations)
                .WithRequired(e => e.Student)
                .HasForeignKey(e => e.student_user_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Teacher>()
                .Property(e => e.user_id)
                .IsUnicode(false);

            modelBuilder.Entity<Teacher>()
                .HasMany(e => e.AdministrativeClasses)
                .WithOptional(e => e.Teacher)
                .HasForeignKey(e => e.advisor_user_id);

            modelBuilder.Entity<Teacher>()
                .HasMany(e => e.CourseOfferings)
                .WithRequired(e => e.Teacher)
                .HasForeignKey(e => e.teacher_user_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .Property(e => e.user_id)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.password_hash)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasOptional(e => e.Profile)
                .WithRequired(e => e.User);

            modelBuilder.Entity<User>()
                .HasOptional(e => e.Student)
                .WithRequired(e => e.User);

            modelBuilder.Entity<User>()
                .HasOptional(e => e.Teacher)
                .WithRequired(e => e.User);
        }
    }
}
