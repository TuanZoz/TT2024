using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebBanHang.Models;

public partial class QuanLyBanHangOnline4Context : DbContext
{
    public QuanLyBanHangOnline4Context()
    {
    }

    public QuanLyBanHangOnline4Context(DbContextOptions<QuanLyBanHangOnline4Context> options)
        : base(options)
    {
    }

    public virtual DbSet<HoaDon> HoaDon { get; set; }

    public virtual DbSet<HoaDonChiTiet> HoaDonChiTiet { get; set; }

    public virtual DbSet<KhachHang> KhachHang { get; set; }

    public virtual DbSet<LoaiSanPham> LoaiSanPham { get; set; }

    public virtual DbSet<NhapKho> NhapKho { get; set; }

    public virtual DbSet<SanPham> SanPham { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DucTuan;Initial Catalog=QuanLyBanHangOnline4;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HoaDon__3214EC27B79A3FC8");

            entity.ToTable("HoaDon");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .HasColumnName("ID");
            entity.Property(e => e.DiaChi).HasMaxLength(100);
            entity.Property(e => e.GhiChu).HasMaxLength(100);
            entity.Property(e => e.Idkh)
                .HasMaxLength(10)
                .HasColumnName("IDKH");
            entity.Property(e => e.NgayDat).HasColumnType("datetime");
            entity.Property(e => e.NgayGiao).HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasMaxLength(20);

            entity.HasOne(d => d.IdkhNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.Idkh)
                .HasConstraintName("FK__HoaDon__IDKH__5165187F");
        });

        modelBuilder.Entity<HoaDonChiTiet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HoaDonCh__3214EC2774F41AF4");

            entity.ToTable("HoaDonChiTiet");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .HasColumnName("ID");
            entity.Property(e => e.Idhd)
                .HasMaxLength(10)
                .HasColumnName("IDHD");
            entity.Property(e => e.Idsp)
                .HasMaxLength(10)
                .HasColumnName("IDSP");

            entity.HasOne(d => d.IdhdNavigation).WithMany(p => p.HoaDonChiTiets)
                .HasForeignKey(d => d.Idhd)
                .HasConstraintName("FK__HoaDonChiT__IDHD__6383C8BA");

            entity.HasOne(d => d.IdspNavigation).WithMany(p => p.HoaDonChiTiets)
                .HasForeignKey(d => d.Idsp)
                .HasConstraintName("FK__HoaDonChiT__IDSP__628FA481");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.D).HasName("PK__KhachHan__3BD019A9E1BC656C");

            entity.ToTable("KhachHang");

            entity.Property(e => e.D).HasMaxLength(10);
            entity.Property(e => e.DiaChi).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.NgaySinh).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(10);
            entity.Property(e => e.Sdt)
                .HasMaxLength(10)
                .HasColumnName("SDT");
        });

        modelBuilder.Entity<LoaiSanPham>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LoaiSanP__3214EC272554389A");

            entity.ToTable("LoaiSanPham");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .HasColumnName("ID");
            entity.Property(e => e.MauSac).HasMaxLength(20);
            entity.Property(e => e.MoTa).HasMaxLength(50);
            entity.Property(e => e.Size).HasMaxLength(10);
            entity.Property(e => e.TenLoai).HasMaxLength(50);
        });

        modelBuilder.Entity<NhapKho>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NhapKho__3214EC27DE0C8729");

            entity.ToTable("NhapKho");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .HasColumnName("ID");
            entity.Property(e => e.Idsp)
                .HasMaxLength(10)
                .HasColumnName("IDSP");
            entity.Property(e => e.NgayNhap).HasColumnType("datetime");

            entity.HasOne(d => d.IdspNavigation).WithMany(p => p.NhapKhos)
                .HasForeignKey(d => d.Idsp)
                .HasConstraintName("FK__NhapKho__IDSP__66603565");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SanPham__3214EC272120989A");

            entity.ToTable("SanPham");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .HasColumnName("ID");
            entity.Property(e => e.HinhAnh).HasMaxLength(50);
            entity.Property(e => e.Idloai)
                .HasMaxLength(10)
                .HasColumnName("IDLoai");
            entity.Property(e => e.MoTa).HasMaxLength(100);
            entity.Property(e => e.TenSp)
                .HasMaxLength(50)
                .HasColumnName("TenSP");

            entity.HasOne(d => d.IdloaiNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.Idloai)
                .HasConstraintName("FK__SanPham__IDLoai__5FB337D6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
