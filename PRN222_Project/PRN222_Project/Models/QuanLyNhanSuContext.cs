using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PRN222_Project.Models;

public partial class QuanLyNhanSuContext : DbContext
{
    public QuanLyNhanSuContext()
    {
    }

    public QuanLyNhanSuContext(DbContextOptions<QuanLyNhanSuContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Checkout> Checkouts { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Emailconfig> Emailconfigs { get; set; }

    public virtual DbSet<EmployeeDataLog> EmployeeDataLogs { get; set; }

    public virtual DbSet<Leaveconfig> Leaveconfigs { get; set; }

    public virtual DbSet<Leaverequest> Leaverequests { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SalaryLevel> SalaryLevels { get; set; }

    public virtual DbSet<SalaryLog> SalaryLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=QuanLyNhanSu;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Checkout>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__checkout__3214EC076F7069BD");

            entity.ToTable("checkout");

            entity.Property(e => e.ActionType).HasMaxLength(10);
            entity.Property(e => e.CheckInTime).HasColumnType("datetime");
            entity.Property(e => e.CheckOutTime).HasColumnType("datetime");
            entity.Property(e => e.EmailSent).HasColumnName("Email_Sent");
            entity.Property(e => e.Status).HasMaxLength(10);

            entity.HasOne(d => d.User).WithMany(p => p.Checkouts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__checkout__UserId__48CFD27E");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__departme__3214EC0787F0865E");

            entity.ToTable("department");

            entity.Property(e => e.DepartmentName).HasMaxLength(255);
            entity.Property(e => e.ParentDepartmentId).HasColumnName("ParentDepartmentID");
            entity.Property(e => e.Status).HasMaxLength(10);
        });

        modelBuilder.Entity<Emailconfig>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__emailcon__3214EC07F404D771");

            entity.ToTable("emailconfig");

            entity.Property(e => e.LastEditConfig)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.User).WithMany(p => p.Emailconfigs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__emailconf__UserI__4222D4EF");
        });

        modelBuilder.Entity<EmployeeDataLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__employee__3214EC0780D627D2");

            entity.ToTable("employee_data_log");

            entity.Property(e => e.ActionTime)
                .HasColumnType("datetime")
                .HasColumnName("Action_Time");
            entity.Property(e => e.ActionType)
                .HasMaxLength(10)
                .HasColumnName("Action_Type");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("File_Name");
            entity.Property(e => e.TotalRecords).HasColumnName("Total_Records");

            entity.HasOne(d => d.PerformedByNavigation).WithMany(p => p.EmployeeDataLogs)
                .HasForeignKey(d => d.PerformedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__employee___Perfo__4CA06362");
        });

        modelBuilder.Entity<Leaveconfig>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__leavecon__3214EC079ACC81F8");

            entity.ToTable("leaveconfig");

            entity.HasOne(d => d.User).WithMany(p => p.Leaveconfigs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__leaveconf__UserI__3A81B327");
        });

        modelBuilder.Entity<Leaverequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__leavereq__3214EC0756F8B684");

            entity.ToTable("leaverequest");

            entity.Property(e => e.ApprovedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(10);

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.LeaverequestApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__leaverequ__Appro__3F466844");

            entity.HasOne(d => d.User).WithMany(p => p.LeaverequestUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__leaverequ__UserI__3E52440B");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__log__3214EC07E31AB4D5");

            entity.ToTable("log");

            entity.Property(e => e.Action).HasMaxLength(255);
            entity.Property(e => e.Timestamp).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Logs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__log__UserId__44FF419A");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__position__3214EC071A42CDCA");

            entity.ToTable("positions");

            entity.HasIndex(e => e.PositionName, "UQ__position__E46AEF4234C78AC0").IsUnique();

            entity.Property(e => e.PositionName).HasMaxLength(100);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__role__3214EC0702E5B28C");

            entity.ToTable("role");

            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<SalaryLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__salary_l__3214EC0768531738");

            entity.ToTable("salary_levels");

            entity.Property(e => e.DailySalary)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("Daily_Salary");
            entity.Property(e => e.MonthlySalary)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Monthly_Salary");
        });

        modelBuilder.Entity<SalaryLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__salary_l__3214EC0776B4178E");

            entity.ToTable("salary_logs");

            entity.Property(e => e.Bonus).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EmployeeId).HasColumnName("Employee_Id");
            entity.Property(e => e.InvalidDays).HasColumnName("Invalid_Days");
            entity.Property(e => e.ProcessedAt)
                .HasColumnType("datetime")
                .HasColumnName("Processed_At");
            entity.Property(e => e.ProcessedBy).HasColumnName("Processed_By");
            entity.Property(e => e.SalaryLevel)
                .HasMaxLength(255)
                .HasColumnName("Salary_Level");
            entity.Property(e => e.TotalDays).HasColumnName("Total_Days");
            entity.Property(e => e.TotalSalary)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Total_Salary");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_At");
            entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");
            entity.Property(e => e.ValidDays).HasColumnName("Valid_Days");

            entity.HasOne(d => d.Employee).WithMany(p => p.SalaryLogs)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__salary_lo__Emplo__37A5467C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3214EC076E52520B");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__A9D1053480EF92AB").IsUnique();

            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.EmploymentType).HasMaxLength(20);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.SalaryLevelId).HasColumnName("Salary_Level_Id");
            entity.Property(e => e.Status).HasMaxLength(10);

            entity.HasOne(d => d.Department).WithMany(p => p.Users)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__users__Departmen__31EC6D26");

            entity.HasOne(d => d.Position).WithMany(p => p.Users)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("FK__users__PositionI__34C8D9D1");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__users__RoleID__32E0915F");

            entity.HasOne(d => d.SalaryLevel).WithMany(p => p.Users)
                .HasForeignKey(d => d.SalaryLevelId)
                .HasConstraintName("FK__users__Salary_Le__33D4B598");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
