using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CollectionKeepersAPIV1.Models;

public partial class CollectionsDbContext : DbContext
{
    public CollectionsDbContext()
    {
    }

    public CollectionsDbContext(DbContextOptions<CollectionsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblAttribute> TblAttributes { get; set; }

    public virtual DbSet<TblAttributeValue> TblAttributeValues { get; set; }

    public virtual DbSet<TblCollection> TblCollections { get; set; }

    public virtual DbSet<TblCollectionEntry> TblCollectionEntries { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblAttribute>(entity =>
        {
            entity.HasKey(e => e.FldAttributeId).HasName("PK__tbl_Attr__F63C1BA14CF72464");

            entity.ToTable("tbl_Attributes");

            entity.Property(e => e.FldAttributeId).HasColumnName("fld_AttributeID");
            entity.Property(e => e.FldAttributeName)
                .HasMaxLength(30)
                .HasColumnName("fld_AttributeName");
            entity.Property(e => e.FldCollectionId).HasColumnName("fld_CollectionID");

            entity.HasOne(d => d.FldCollection).WithMany(p => p.TblAttributes)
                .HasForeignKey(d => d.FldCollectionId)
                .HasConstraintName("FK__tbl_Attri__fld_C__3E52440B");
        });

        modelBuilder.Entity<TblAttributeValue>(entity =>
        {
            entity.HasKey(e => e.FldAttributeValueId).HasName("PK__tbl_Attr__E53410CD41B95370");

            entity.ToTable("tbl_AttributeValue");

            entity.Property(e => e.FldAttributeValueId).HasColumnName("fld_AttributeValueID");
            entity.Property(e => e.FldAttributeId).HasColumnName("fld_AttributeID");
            entity.Property(e => e.FldCollectionEntryId).HasColumnName("fld_CollectionEntryID");
            entity.Property(e => e.FldValue)
                .HasMaxLength(100)
                .HasColumnName("fld_Value");

            entity.HasOne(d => d.FldCollectionEntry).WithMany(p => p.TblAttributeValues)
                .HasForeignKey(d => d.FldCollectionEntryId)
                .HasConstraintName("FK__tbl_Attri__fld_C__412EB0B6");
        });

        modelBuilder.Entity<TblCollection>(entity =>
        {
            entity.HasKey(e => e.FldCollectionId).HasName("PK__tbl_Coll__B1FC26F9CA2EDC1D");

            entity.ToTable("tbl_Collection");

            entity.Property(e => e.FldCollectionId).HasColumnName("fld_CollectionID");
            entity.Property(e => e.FldCollectionDescription)
                .HasMaxLength(100)
                .HasColumnName("fld_CollectionDescription");
            entity.Property(e => e.FldCollectionName)
                .HasMaxLength(30)
                .HasColumnName("fld_CollectionName");
            entity.Property(e => e.FldCollectionThumbnail)
                .HasMaxLength(500)
                .HasColumnName("fld_CollectionThumbnail");
            entity.Property(e => e.FldIsPrivate).HasColumnName("fld_IsPrivate");
            entity.Property(e => e.FldUserId).HasColumnName("fld_UserID");

            entity.HasOne(d => d.FldUser).WithMany(p => p.TblCollections)
                .HasForeignKey(d => d.FldUserId)
                .HasConstraintName("FK__tbl_Colle__fld_U__398D8EEE");
        });

        modelBuilder.Entity<TblCollectionEntry>(entity =>
        {
            entity.HasKey(e => e.FldCollectionEntryId).HasName("PK__tbl_Coll__802C2EB200331EA7");

            entity.ToTable("tbl_CollectionEntry");

            entity.Property(e => e.FldCollectionEntryId).HasColumnName("fld_CollectionEntryID");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.FldUserId).HasName("PK__tbl_User__C851D2E64D12E2DE");

            entity.ToTable("tbl_User");

            entity.Property(e => e.FldUserId).HasColumnName("fld_UserID");
            entity.Property(e => e.FldEmail)
                .HasMaxLength(50)
                .HasColumnName("fld_Email");
            entity.Property(e => e.FldPassword)
                .HasMaxLength(30)
                .HasColumnName("fld_Password");
            entity.Property(e => e.FldUsername)
                .HasMaxLength(30)
                .HasColumnName("fld_Username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
