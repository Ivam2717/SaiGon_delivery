using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Saigon_delivery.Models;

namespace Saigon_delivery.Data;

public partial class SaigonDeliveryContext : DbContext
{
    public SaigonDeliveryContext()
    {
    }

    public SaigonDeliveryContext(DbContextOptions<SaigonDeliveryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<ShipperRating> ShipperRatings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer("Server=DESKTOP-L1R9Q1N\\SQLEXPRESS;Database=SaigonDeliveryDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC07907A6160");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.DeliveryAddress).HasMaxLength(255);
            entity.Property(e => e.Distance).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.ReceiverName).HasMaxLength(100);
            entity.Property(e => e.ReceiverPhone).HasMaxLength(20);
            entity.Property(e => e.ShipFee).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Customer).WithMany(p => p.OrderCustomers)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Customer");

            entity.HasOne(d => d.Shipper).WithMany(p => p.OrderShippers)
                .HasForeignKey(d => d.ShipperId)
                .HasConstraintName("FK_Orders_Shipper");
        });

        modelBuilder.Entity<ShipperRating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ShipperR__3214EC07846A7F55");

            entity.HasIndex(e => e.OrderId, "UQ__ShipperR__C3905BCE7E922838").IsUnique();

            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Customer).WithMany(p => p.ShipperRatingCustomers)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ratings_Customer");

            entity.HasOne(d => d.Order).WithOne(p => p.ShipperRating)
                .HasForeignKey<ShipperRating>(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ratings_Order");

            entity.HasOne(d => d.Shipper).WithMany(p => p.ShipperRatingShippers)
                .HasForeignKey(d => d.ShipperId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ratings_Shipper");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC078E3C6FED");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105343EC8D706").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
