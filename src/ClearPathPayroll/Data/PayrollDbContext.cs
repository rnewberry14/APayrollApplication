using Microsoft.EntityFrameworkCore;
using ClearPathPayroll.Domain;

namespace ClearPathPayroll.Data;

public class PayrollDbContext : DbContext
{
    public PayrollDbContext(DbContextOptions<PayrollDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<Company> Companies { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<PaySchedule> PaySchedules { get; set; }
    public DbSet<PayrollRun> PayrollRuns { get; set; }
    public DbSet<PayrollRunEmployee> PayrollRunEmployees { get; set; }
    public DbSet<EarningLine> EarningLines { get; set; }
    public DbSet<DeductionLine> DeductionLines { get; set; }
    public DbSet<TaxLine> TaxLines { get; set; }
    public DbSet<EmployerTaxLine> EmployerTaxLines { get; set; }
    public DbSet<NetPayLine> NetPayLines { get; set; }
    public DbSet<AuditLogEntry> AuditLogEntries { get; set; }
    public DbSet<EmployeeBankAccount> EmployeeBankAccounts { get; set; }
    public DbSet<CompanyFundingAccount> CompanyFundingAccounts { get; set; }
    public DbSet<DirectDepositBatch> DirectDepositBatches { get; set; }
    public DbSet<DirectDepositItem> DirectDepositItems { get; set; }

    // Add DbSets here as entities are created

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Company configuration
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(c => c.CompanyId);
            entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Employee configuration
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId);
            entity.HasOne(e => e.Company)
                  .WithMany()
                  .HasForeignKey(e => e.CompanyId);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.HourlyRate).HasPrecision(18, 2);
            entity.Property(e => e.AnnualSalary).HasPrecision(18, 2);
        });

        // PaySchedule configuration
        modelBuilder.Entity<PaySchedule>(entity =>
        {
            entity.HasKey(p => p.PayScheduleId);
            entity.HasOne(p => p.Company)
                  .WithMany()
                  .HasForeignKey(p => p.CompanyId);
        });

        // PayrollRun configuration
        modelBuilder.Entity<PayrollRun>(entity =>
        {
            entity.HasKey(p => p.PayrollRunId);
            entity.HasOne(p => p.Company)
                  .WithMany()
                  .HasForeignKey(p => p.CompanyId);
            entity.HasOne(p => p.PaySchedule)
                  .WithMany()
                  .HasForeignKey(p => p.PayScheduleId);
            entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(p => p.TotalGrossPay).HasPrecision(18, 2);
            entity.Property(p => p.TotalEmployeeTaxes).HasPrecision(18, 2);
            entity.Property(p => p.TotalEmployerTaxes).HasPrecision(18, 2);
            entity.Property(p => p.TotalDeductions).HasPrecision(18, 2);
            entity.Property(p => p.TotalNetPay).HasPrecision(18, 2);
        });

        // PayrollRunEmployee configuration
        modelBuilder.Entity<PayrollRunEmployee>(entity =>
        {
            entity.HasKey(p => p.PayrollRunEmployeeId);
            entity.HasOne(p => p.PayrollRun)
                  .WithMany(pr => pr.PayrollRunEmployees)
                  .HasForeignKey(p => p.PayrollRunId);
            entity.HasOne(p => p.Employee)
                  .WithMany()
                  .HasForeignKey(p => p.EmployeeId);
            entity.Property(p => p.GrossPay).HasPrecision(18, 2);
            entity.Property(p => p.TotalDeductions).HasPrecision(18, 2);
            entity.Property(p => p.TotalTaxes).HasPrecision(18, 2);
            entity.Property(p => p.NetPay).HasPrecision(18, 2);
        });

        // EarningLine configuration
        modelBuilder.Entity<EarningLine>(entity =>
        {
            entity.HasKey(e => e.EarningLineId);
            entity.HasOne(e => e.PayrollRunEmployee)
                  .WithMany(pre => pre.EarningLines)
                  .HasForeignKey(e => e.PayrollRunEmployeeId);
            entity.Property(e => e.Hours).HasPrecision(18, 2);
            entity.Property(e => e.Rate).HasPrecision(18, 2);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });

        // DeductionLine configuration
        modelBuilder.Entity<DeductionLine>(entity =>
        {
            entity.HasKey(d => d.DeductionLineId);
            entity.HasOne(d => d.PayrollRunEmployee)
                  .WithMany(pre => pre.DeductionLines)
                  .HasForeignKey(d => d.PayrollRunEmployeeId);
            entity.Property(d => d.Amount).HasPrecision(18, 2);
        });

        // TaxLine configuration
        modelBuilder.Entity<TaxLine>(entity =>
        {
            entity.HasKey(t => t.TaxLineId);
            entity.HasOne(t => t.PayrollRunEmployee)
                  .WithMany(pre => pre.TaxLines)
                  .HasForeignKey(t => t.PayrollRunEmployeeId);
            entity.Property(t => t.Amount).HasPrecision(18, 2);
        });

        // EmployerTaxLine configuration
        modelBuilder.Entity<EmployerTaxLine>(entity =>
        {
            entity.HasKey(e => e.EmployerTaxLineId);
            entity.HasOne(e => e.PayrollRun)
                  .WithMany(pr => pr.EmployerTaxLines)
                  .HasForeignKey(e => e.PayrollRunId);
            entity.HasOne(e => e.Employee)
                  .WithMany()
                  .HasForeignKey(e => e.EmployeeId);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });

        // NetPayLine configuration
        modelBuilder.Entity<NetPayLine>(entity =>
        {
            entity.HasKey(n => n.NetPayLineId);
            entity.HasOne(n => n.PayrollRunEmployee)
                  .WithMany(pre => pre.NetPayLines)
                  .HasForeignKey(n => n.PayrollRunEmployeeId);
            entity.Property(n => n.Amount).HasPrecision(18, 2);
        });

        // EmployeeBankAccount configuration
        modelBuilder.Entity<EmployeeBankAccount>(entity =>
        {
            entity.HasKey(e => e.EmployeeBankAccountId);
            entity.HasOne(e => e.Employee)
                  .WithMany()
                  .HasForeignKey(e => e.EmployeeId);
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.RoutingNumberToken).HasMaxLength(256);
            entity.Property(e => e.AccountNumberToken).HasMaxLength(256);
            entity.Property(e => e.Last4).HasMaxLength(4);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // CompanyFundingAccount configuration
        modelBuilder.Entity<CompanyFundingAccount>(entity =>
        {
            entity.HasKey(c => c.CompanyFundingAccountId);
            entity.HasOne(c => c.Company)
                  .WithMany()
                  .HasForeignKey(c => c.CompanyId);
            entity.Property(c => c.BankName).HasMaxLength(100);
            entity.Property(c => c.RoutingNumberToken).HasMaxLength(256);
            entity.Property(c => c.AccountNumberToken).HasMaxLength(256);
            entity.Property(c => c.Last4).HasMaxLength(4);
            entity.Property(c => c.Description).HasMaxLength(200);
            entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(c => c.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // DirectDepositBatch configuration
        modelBuilder.Entity<DirectDepositBatch>(entity =>
        {
            entity.HasKey(b => b.BatchId);
            entity.HasOne(b => b.PayrollRun)
                  .WithMany()
                  .HasForeignKey(b => b.PayrollRunId);
            entity.HasOne(b => b.Company)
                  .WithMany()
                  .HasForeignKey(b => b.CompanyId);
            entity.HasOne(b => b.FundingAccount)
                  .WithMany(f => f.DirectDepositBatches)
                  .HasForeignKey(b => b.CompanyFundingAccountId);
            entity.Property(b => b.TotalAmount).HasPrecision(18, 2);
            entity.Property(b => b.Description).HasMaxLength(200);
            entity.Property(b => b.ExternalBatchReference).HasMaxLength(256);
            entity.Property(b => b.ErrorMessage).HasMaxLength(1000);
            entity.Property(b => b.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // DirectDepositItem configuration
        modelBuilder.Entity<DirectDepositItem>(entity =>
        {
            entity.HasKey(i => i.BatchItemId);
            entity.HasOne(i => i.Batch)
                  .WithMany(b => b.Items)
                  .HasForeignKey(i => i.BatchId);
            entity.HasOne(i => i.Employee)
                  .WithMany()
                  .HasForeignKey(i => i.EmployeeId);
            entity.HasOne(i => i.BankAccount)
                  .WithMany(a => a.DirectDepositItems)
                  .HasForeignKey(i => i.EmployeeBankAccountId);
            entity.Property(i => i.Amount).HasPrecision(18, 2);
            entity.Property(i => i.ExternalItemReference).HasMaxLength(256);
            entity.Property(i => i.ReturnCode).HasMaxLength(10);
            entity.Property(i => i.ReturnDescription).HasMaxLength(500);
        });

        // AuditLogEntry configuration
        modelBuilder.Entity<AuditLogEntry>(entity =>
        {
            entity.HasKey(a => a.AuditLogEntryId);
            entity.HasOne(a => a.PayrollRun)
                  .WithMany()
                  .HasForeignKey(a => a.PayrollRunId);
            entity.Property(a => a.EventType).HasMaxLength(100);
            entity.Property(a => a.Description).HasMaxLength(1000);
        });
    }
}