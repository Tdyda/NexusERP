using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NexusERP.Models;

namespace NexusERP.Data;

public partial class PhmDbContext : DbContext
{
    public PhmDbContext()
    {
    }

    public PhmDbContext(DbContextOptions<PhmDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MtlMaterial> MtlMaterials { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=10.0.60.235\\\\\\\\PHMESDB,49773;Database=PHM-PRC;TrustServerCertificate=True;User Id=perla_tdyda;Password=E(JnIO+)*2Kd");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MtlMaterial>(entity =>
        {
            entity.HasKey(e => e.MaterialId);

            entity.ToTable("MTL_Material");

            entity.HasIndex(e => e.CsbMaterialId, "MS20230602_05");

            entity.HasIndex(e => e.MaterialId, "MTL_Material_MaterialId").IsUnique();

            entity.Property(e => e.MaterialId)
                .HasMaxLength(20)
                .HasColumnName("material_id");
            entity.Property(e => e.ActiveFlag).HasColumnName("ACTIVE_FLAG");
            entity.Property(e => e.AlarmQtyMin).HasColumnName("alarm_qty_min");
            entity.Property(e => e.Api)
                .HasMaxLength(10)
                .HasColumnName("api");
            entity.Property(e => e.AreaId).HasColumnName("area_id");
            entity.Property(e => e.BrandProduct)
                .HasMaxLength(50)
                .HasColumnName("brand_product");
            entity.Property(e => e.Comment)
                .HasMaxLength(255)
                .HasColumnName("comment");
            entity.Property(e => e.CommentClient)
                .HasMaxLength(255)
                .HasColumnName("comment_client");
            entity.Property(e => e.CommentProduction)
                .HasMaxLength(255)
                .HasColumnName("comment_production");
            entity.Property(e => e.ContainerTrack)
                .HasMaxLength(10)
                .HasColumnName("container_track");
            entity.Property(e => e.CsbMaterialId)
                .HasMaxLength(20)
                .HasColumnName("csb_material_id");
            entity.Property(e => e.DefTransfer)
                .HasMaxLength(20)
                .HasColumnName("DEF_TRANSFER");
            entity.Property(e => e.DefTransferCoef).HasColumnName("DEF_TRANSFER_COEF");
            entity.Property(e => e.Ean)
                .HasMaxLength(128)
                .HasColumnName("ean");
            entity.Property(e => e.ExpirationRule).HasColumnName("expiration_rule");
            entity.Property(e => e.ExpirationRuleMax).HasColumnName("expiration_rule_max");
            entity.Property(e => e.ExpirationRuleMin).HasColumnName("expiration_rule_min");
            entity.Property(e => e.ExpirationRuleOld).HasColumnName("expiration_rule_old");
            entity.Property(e => e.ExtLotFlag)
                .HasMaxLength(10)
                .HasColumnName("ext_lot_flag");
            entity.Property(e => e.Flags)
                .HasMaxLength(20)
                .HasColumnName("flags");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.InventoryUom)
                .HasMaxLength(10)
                .HasColumnName("inventory_uom");
            entity.Property(e => e.MaterialDesc)
                .HasMaxLength(255)
                .HasColumnName("material_desc");
            entity.Property(e => e.MaterialDescDeEng)
                .HasMaxLength(128)
                .HasColumnName("material_desc_de_eng");
            entity.Property(e => e.MaterialDescExtra)
                .HasMaxLength(128)
                .HasColumnName("material_desc_extra");
            entity.Property(e => e.MaterialDescShort)
                .HasMaxLength(128)
                .HasColumnName("material_desc_short");
            entity.Property(e => e.MaterialType)
                .HasMaxLength(20)
                .HasColumnName("material_type");
            entity.Property(e => e.MsdsName)
                .HasMaxLength(20)
                .HasColumnName("msds_name");
            entity.Property(e => e.MsdsVer)
                .HasMaxLength(20)
                .HasColumnName("msds_ver");
            entity.Property(e => e.MtlGroup).HasColumnName("mtl_group");
            entity.Property(e => e.MtlSubgroup).HasColumnName("mtl_subgroup");
            entity.Property(e => e.OrderUomS)
                .HasMaxLength(10)
                .HasColumnName("order_uom_s");
            entity.Property(e => e.OrderUomZ)
                .HasMaxLength(10)
                .HasColumnName("order_uom_z");
            entity.Property(e => e.OrgnCode).HasColumnName("orgn_code");
            entity.Property(e => e.PckgGroup).HasColumnName("pckg_group");
            entity.Property(e => e.PriceUomS)
                .HasMaxLength(10)
                .HasColumnName("price_uom_s");
            entity.Property(e => e.PriceUomZ)
                .HasMaxLength(10)
                .HasColumnName("price_uom_z");
            entity.Property(e => e.ProdQcAttribute)
                .HasMaxLength(20)
                .HasColumnName("prod_qc_attribute");
            entity.Property(e => e.ProdQcStatus)
                .HasMaxLength(20)
                .HasColumnName("prod_qc_status");
            entity.Property(e => e.ProductType)
                .HasMaxLength(20)
                .HasColumnName("product_type");
            entity.Property(e => e.ReceiptClass)
                .HasMaxLength(20)
                .HasColumnName("receipt_class");
            entity.Property(e => e.RegCertNo1)
                .HasMaxLength(255)
                .HasColumnName("reg_cert_no1");
            entity.Property(e => e.RegCertNo2)
                .HasMaxLength(255)
                .HasColumnName("reg_cert_no2");
            entity.Property(e => e.SopName)
                .HasMaxLength(60)
                .HasColumnName("sop_name");
            entity.Property(e => e.SopVer)
                .HasMaxLength(20)
                .HasColumnName("sop_ver");
            entity.Property(e => e.SpecName)
                .HasMaxLength(20)
                .HasColumnName("spec_name");
            entity.Property(e => e.SpecVer)
                .HasMaxLength(20)
                .HasColumnName("spec_ver");
            entity.Property(e => e.StorageCond)
                .HasMaxLength(255)
                .HasColumnName("storage_cond");
            entity.Property(e => e.TMaterialId)
                .HasMaxLength(20)
                .HasColumnName("t_material_id");
            entity.Property(e => e.TradeItemIdNo)
                .HasMaxLength(255)
                .HasColumnName("TRADE_ITEM_ID_NO");
            entity.Property(e => e.WhseGeoloc).HasColumnName("whse_geoloc");
        });
        modelBuilder.HasSequence("WzSerial");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
