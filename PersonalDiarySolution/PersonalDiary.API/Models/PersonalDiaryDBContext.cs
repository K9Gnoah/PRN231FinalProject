using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PersonalDiary.API.Models
{
    public partial class PersonalDiaryDBContext : DbContext
    {
        public PersonalDiaryDBContext()
        {
        }

        public PersonalDiaryDBContext(DbContextOptions<PersonalDiaryDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<DiaryEntry> DiaryEntries { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("MyContr"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.Content).HasColumnType("ntext");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.GuestName).HasMaxLength(50);

                entity.HasOne(d => d.Entry)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.EntryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Comments__EntryI__5CD6CB2B");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Comments__UserId__5DCAEF64");
            });

            modelBuilder.Entity<DiaryEntry>(entity =>
            {
                entity.HasKey(e => e.EntryId)
                    .HasName("PK__DiaryEnt__F57BD2F7C8F2A21D");

                entity.Property(e => e.Content).HasColumnType("ntext");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsPublic).HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Mood).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(200);

                entity.Property(e => e.Weather).HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DiaryEntries)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DiaryEntr__UserI__52593CB8");

                entity.HasMany(d => d.Tags)
                    .WithMany(p => p.Entries)
                    .UsingEntity<Dictionary<string, object>>(
                        "DiaryEntryTag",
                        l => l.HasOne<Tag>().WithMany().HasForeignKey("TagId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__DiaryEntr__TagId__59063A47"),
                        r => r.HasOne<DiaryEntry>().WithMany().HasForeignKey("EntryId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__DiaryEntr__Entry__5812160E"),
                        j =>
                        {
                            j.HasKey("EntryId", "TagId").HasName("PK__DiaryEnt__232C1D6D9200E50B");

                            j.ToTable("DiaryEntryTags");
                        });
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasIndex(e => e.TagName, "UQ__Tags__BDE0FD1D8926F347")
                    .IsUnique();

                entity.Property(e => e.TagName).HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username, "UQ__Users__536C85E48D2DE83E")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__Users__A9D10534153000F9")
                    .IsUnique();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FullName).HasMaxLength(100);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");

                entity.Property(e => e.PasswordHash).HasMaxLength(255);

                entity.Property(e => e.Username).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
