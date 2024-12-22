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

        public virtual DbSet<AdminClass> AdminClasses { get; set; }
        public virtual DbSet<CourseOffering> CourseOfferings { get; set; }
        public virtual DbSet<Cours> Courses { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Semester> Semesters { get; set; }
        public virtual DbSet<StudentCourseRegistration> StudentCourseRegistrations { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<TrainingProgramCours> TrainingProgramCourses { get; set; }
        public virtual DbSet<TrainingProgram> TrainingPrograms { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminClass>()
                .Property(e => e.class_id)
                .IsUnicode(false);

            modelBuilder.Entity<AdminClass>()
                .Property(e => e.department_id)
                .IsUnicode(false);

            modelBuilder.Entity<AdminClass>()
                .Property(e => e.advisor_teacher_id)
                .IsUnicode(false);

            modelBuilder.Entity<AdminClass>()
                .HasMany(e => e.Students)
                .WithOptional(e => e.AdminClass)
                .HasForeignKey(e => e.administrative_class_id);

            modelBuilder.Entity<CourseOffering>()
                .Property(e => e.course_id)
                .IsUnicode(false);

            modelBuilder.Entity<CourseOffering>()
                .Property(e => e.teacher_user_id)
                .IsUnicode(false);

            modelBuilder.Entity<CourseOffering>()
                .HasMany(e => e.StudentCourseRegistrations)
                .WithRequired(e => e.CourseOffering)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Cours>()
                .Property(e => e.course_id)
                .IsUnicode(false);

            modelBuilder.Entity<Cours>()
                .Property(e => e.department_id)
                .IsUnicode(false);

            modelBuilder.Entity<Cours>()
                .Property(e => e.prerequisite_course_id)
                .IsUnicode(false);

            modelBuilder.Entity<Cours>()
                .HasMany(e => e.CourseOfferings)
                .WithRequired(e => e.Cours)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Cours>()
                .HasMany(e => e.Courses1)
                .WithOptional(e => e.Cours1)
                .HasForeignKey(e => e.prerequisite_course_id);

            modelBuilder.Entity<Cours>()
                .HasMany(e => e.TrainingProgramCourses)
                .WithRequired(e => e.Cours)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .Property(e => e.department_id)
                .IsUnicode(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.AdminClasses)
                .WithRequired(e => e.Department)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Courses)
                .WithRequired(e => e.Department)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Teachers)
                .WithRequired(e => e.Department)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.TrainingPrograms)
                .WithRequired(e => e.Department)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Grade>()
                .Property(e => e.score)
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
                .Property(e => e.student_id)
                .IsUnicode(false);

            modelBuilder.Entity<StudentCourseRegistration>()
                .HasMany(e => e.Grades)
                .WithRequired(e => e.StudentCourseRegistration)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Student>()
                .Property(e => e.user_id)
                .IsUnicode(false);

            modelBuilder.Entity<Student>()
                .Property(e => e.program_id)
                .IsUnicode(false);

            modelBuilder.Entity<Student>()
                .Property(e => e.administrative_class_id)
                .IsUnicode(false);

            modelBuilder.Entity<Student>()
                .HasMany(e => e.StudentCourseRegistrations)
                .WithRequired(e => e.Student)
                .HasForeignKey(e => e.student_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Teacher>()
                .Property(e => e.user_id)
                .IsUnicode(false);

            modelBuilder.Entity<Teacher>()
                .Property(e => e.department_id)
                .IsUnicode(false);

            modelBuilder.Entity<Teacher>()
                .HasMany(e => e.AdminClasses)
                .WithOptional(e => e.Teacher)
                .HasForeignKey(e => e.advisor_teacher_id);

            modelBuilder.Entity<Teacher>()
                .HasMany(e => e.CourseOfferings)
                .WithRequired(e => e.Teacher)
                .HasForeignKey(e => e.teacher_user_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingProgramCours>()
                .Property(e => e.program_id)
                .IsUnicode(false);

            modelBuilder.Entity<TrainingProgramCours>()
                .Property(e => e.course_id)
                .IsUnicode(false);

            modelBuilder.Entity<TrainingProgram>()
                .Property(e => e.program_id)
                .IsUnicode(false);

            modelBuilder.Entity<TrainingProgram>()
                .Property(e => e.department_id)
                .IsUnicode(false);

            modelBuilder.Entity<TrainingProgram>()
                .Property(e => e.version)
                .IsUnicode(false);

            modelBuilder.Entity<TrainingProgram>()
                .HasMany(e => e.Students)
                .WithRequired(e => e.TrainingProgram)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingProgram>()
                .HasMany(e => e.TrainingProgramCourses)
                .WithRequired(e => e.TrainingProgram)
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
