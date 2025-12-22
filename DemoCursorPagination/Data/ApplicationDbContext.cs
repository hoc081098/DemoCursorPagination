using System;
using System.Collections.Generic;
using DemoCursorPagination.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoCursorPagination.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<UserNotes> UserNotes { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<UserNotes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_notes_pkey");

            entity.ToTable("user_notes", "notes");

            entity.HasIndex(e => new { e.NoteDate, e.Id }, "idx_user_notes_note_date_id").IsDescending();

            entity.HasIndex(e => e.UserId, "idx_user_notes_user_id");

            entity.Property(e => e.Id)
                .HasValueGenerator<GuidV7ValueGenerator>()
                .HasColumnName("id");
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .HasColumnName("note");
            entity.Property(e => e.NoteDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("note_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserNotes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_notes__user");
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", "identity");

            entity.Property(e => e.Id)
                .HasValueGenerator<GuidV7ValueGenerator>()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
