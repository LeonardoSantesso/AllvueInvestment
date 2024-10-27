using Investment.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Investment.DAL.Data;

public class InvestmentDbContext : DbContext
{
    public DbSet<StockLot> StockLots { get; set; }
    public DbSet<SaleRecord> SaleRecords { get; set; }
    
    public InvestmentDbContext(DbContextOptions<InvestmentDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockLot>(entity =>
        {
            entity.ToTable("stock_lot");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.OriginalShares).HasColumnName("original_shares").IsRequired();
            entity.Property(e => e.Shares).HasColumnName("shares").IsRequired();
            entity.Property(e => e.PricePerShare).HasColumnName("price_per_share").IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.PurchaseDate).HasColumnName("purchase_date").IsRequired().HasColumnType("datetime");

            entity.HasMany(e => e.StockLotSales)
                .WithOne(s => s.StockLot)
                .HasForeignKey(s => s.StockLotId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SaleRecord>(entity =>
        {
            entity.ToTable("sale_record");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.SaleDate).HasColumnName("sale_date").IsRequired();
            entity.Property(e => e.TotalProfit).HasColumnName("total_profit").HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalCost).HasColumnName("total_cost").HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalSaleValue).HasColumnName("total_sale_value").HasColumnType("decimal(18,2)");
            entity.Property(e => e.CostBasisPerSoldShare).HasColumnName("cost_basis_per_sold_share").HasColumnType("decimal(18,2)");

            entity.HasMany(e => e.StockLotSales)
                .WithOne(s => s.SaleRecord)
                .HasForeignKey(s => s.SaleRecordId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StockLotSale>(entity =>
        {
            entity.ToTable("stock_lot_sale");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.SharesSold).HasColumnName("shares_sold").IsRequired();
            entity.Property(e => e.CostBasis).HasColumnName("cost_basis").HasColumnType("decimal(18,2)");
            entity.Property(e => e.StockLotId).HasColumnName("stock_lot_id").IsRequired();
            entity.Property(e => e.SaleRecordId).HasColumnName("sale_record_id").IsRequired();
        });

        // Add initial default data
        modelBuilder.Entity<StockLot>().HasData(
            new StockLot { Id = 1, OriginalShares = 100, Shares = 100, PricePerShare = 20, PurchaseDate = new DateTime(2024, 01, 01)},
            new StockLot { Id = 2, OriginalShares = 150, Shares = 150, PricePerShare = 30, PurchaseDate = new DateTime(2024, 02, 01)},
            new StockLot { Id = 3, OriginalShares = 120, Shares = 120, PricePerShare = 10, PurchaseDate = new DateTime(2024, 03, 01)}
        );

        base.OnModelCreating(modelBuilder);
    }
}
