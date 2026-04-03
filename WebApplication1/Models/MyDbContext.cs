using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrator> Administrators { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<RecordingSlot> RecordingSlots { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<ServicesClinic> ServicesClinics { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=WIN-E35DNA7AABF\\SQLEXPRESS;Database=MedicalClinicDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__ADMINIST__59AF14B588DD93D6");

            entity.ToTable("ADMINISTRATORS");

            entity.HasIndex(e => e.Login, "UQ__ADMINIST__E39E2665D83E90DD").IsUnique();

            entity.Property(e => e.AdminId).HasColumnName("ADMIN_ID");
            entity.Property(e => e.FullName).HasColumnName("FULL_NAME");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("LOGIN");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("PASSWORD");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__APPOINTM__49B308C642D265F2");

            entity.ToTable("APPOINTMENTS");

            entity.Property(e => e.AppointmentId)
                .ValueGeneratedNever()
                .HasColumnName("APPOINTMENT_ID");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATION_DATE");
            entity.Property(e => e.Diagnosis)
                .HasMaxLength(100)
                .HasColumnName("DIAGNOSIS");
            entity.Property(e => e.FinalCost)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("FINAL_COST");
            entity.Property(e => e.Recommendations)
                .HasMaxLength(100)
                .HasColumnName("RECOMMENDATIONS");

            entity.HasOne(d => d.AppointmentNavigation).WithOne(p => p.Appointment)
                .HasForeignKey<Appointment>(d => d.AppointmentId)
                .HasConstraintName("FK__APPOINTME__APPOI__3B75D760");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__DOCTORS__596ABDB00DAC1024");

            entity.ToTable("DOCTORS");

            entity.Property(e => e.DoctorId).HasColumnName("DOCTOR_ID");
            entity.Property(e => e.FullName).HasColumnName("FULL_NAME");
            entity.Property(e => e.SpecializationId).HasColumnName("SPECIALIZATION_ID");

            entity.HasOne(d => d.Specialization).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.SpecializationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DOCTORS__SPECIAL__300424B4");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__PATIENTS__AA0B6068AF52AC4A");

            entity.ToTable("PATIENTS");

            entity.HasIndex(e => e.Email, "UQ__PATIENTS__161CF72404005E83").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ__PATIENTS__D4FA0A26CF809354").IsUnique();

            entity.HasIndex(e => e.InsuranceNumber, "UQ__PATIENTS__FB17B9D2C2BC8AC0").IsUnique();

            entity.Property(e => e.PatientId).HasColumnName("PATIENT_ID");
            entity.Property(e => e.BirthDate).HasColumnName("BIRTH_DATE");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("EMAIL");
            entity.Property(e => e.FullName).HasColumnName("FULL_NAME");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .HasColumnName("GENDER");
            entity.Property(e => e.InsuranceNumber)
                .HasMaxLength(50)
                .HasColumnName("INSURANCE_NUMBER");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("PHONE");
        });

        modelBuilder.Entity<RecordingSlot>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PK__RECORDIN__50DD09B69DDB7980");

            entity.ToTable("RECORDING_SLOT");

            entity.Property(e => e.SlotId).HasColumnName("SLOT_ID");
            entity.Property(e => e.Date).HasColumnName("DATE");
            entity.Property(e => e.DoctorId).HasColumnName("DOCTOR_ID");
            entity.Property(e => e.PatientId).HasColumnName("PATIENT_ID");
            entity.Property(e => e.ServiceId).HasColumnName("SERVICE_ID");
            entity.Property(e => e.StartTime)
                .HasPrecision(2)
                .HasColumnName("START_TIME");

            entity.HasOne(d => d.Doctor).WithMany(p => p.RecordingSlots)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__RECORDING__DOCTO__35BCFE0A");

            entity.HasOne(d => d.Patient).WithMany(p => p.RecordingSlots)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__RECORDING__PATIE__36B12243");

            entity.HasOne(d => d.Service).WithMany(p => p.RecordingSlots)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RECORDING__SERVI__37A5467C");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__SCHEDULE__A9B604880840DA5D");

            entity.ToTable("SCHEDULE");

            entity.Property(e => e.ScheduleId).HasColumnName("SCHEDULE_ID");
            entity.Property(e => e.Date).HasColumnName("DATE");
            entity.Property(e => e.DoctorId).HasColumnName("DOCTOR_ID");
            entity.Property(e => e.EndTime)
                .HasPrecision(2)
                .HasColumnName("END_TIME");
            entity.Property(e => e.StartTime)
                .HasPrecision(2)
                .HasColumnName("START_TIME");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__SCHEDULE__DOCTOR__3E52440B");
        });

        modelBuilder.Entity<ServicesClinic>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__SERVICES__30358F5A3B2DFE7A");

            entity.ToTable("SERVICES_CLINIC");

            entity.Property(e => e.ServiceId).HasColumnName("SERVICE_ID");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("NAME");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("PRICE");
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.SpecializationId).HasName("PK__SPECIALT__57E4BDEBA68329E8");

            entity.ToTable("SPECIALTIES");

            entity.HasIndex(e => e.Name, "UQ__SPECIALT__D9C1FA0094A31396").IsUnique();

            entity.Property(e => e.SpecializationId).HasColumnName("SPECIALIZATION_ID");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("NAME");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
