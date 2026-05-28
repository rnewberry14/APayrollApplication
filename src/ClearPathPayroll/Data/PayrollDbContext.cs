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
    public DbSet<EmployeePayrollField> EmployeePayrollFields { get; set; }
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
    public DbSet<OfficialSourceDocument> OfficialSourceDocuments { get; set; }
    public DbSet<OfficialSourceReviewLog> OfficialSourceReviewLogs { get; set; }
    public DbSet<PayrollItem> PayrollItems { get; set; }
    public DbSet<UserDefinedFieldDefinition> UserDefinedFieldDefinitions { get; set; }
    public DbSet<UserDefinedFieldValue> UserDefinedFieldValues { get; set; }
    public DbSet<ImportBatch> ImportBatches { get; set; }
    public DbSet<ImportRow> ImportRows { get; set; }
    public DbSet<ImportError> ImportErrors { get; set; }
    public DbSet<ImportMapping> ImportMappings { get; set; }
    public DbSet<TaxDepositRecord> TaxDepositRecords { get; set; }
    public DbSet<W2HistoricalRecord> W2HistoricalRecords { get; set; }
    public DbSet<W2ImportBatch> W2ImportBatches { get; set; }
    public DbSet<W2ImportRecord> W2ImportRecords { get; set; }
    public DbSet<W2ImportError> W2ImportErrors { get; set; }

    // Add DbSets here as entities are created

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Company configuration
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(c => c.CompanyId);
            entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(c => c.FutaRatePlaceholder).HasPrecision(9, 4);
            entity.Property(c => c.SutaRate).HasPrecision(9, 4);
            entity.Property(c => c.LocalEmployerTaxRate).HasPrecision(9, 4);
            entity.Property(c => c.County).HasMaxLength(100);
            entity.Property(c => c.LocalTaxLocalityPlaceholder).HasMaxLength(100);
            entity.Property(c => c.SutaEmployerAccountNumberPlaceholder).HasMaxLength(100);
            entity.Property(c => c.SUIN).HasMaxLength(100);
            entity.Property(c => c.SEIN).HasMaxLength(100);
            entity.Property(c => c.StateWithholdingAccountNumberPlaceholder).HasMaxLength(100);
            entity.Property(c => c.LocalTaxAccountNumberPlaceholder).HasMaxLength(100);
            entity.Property(c => c.DepositSchedulePlaceholder).HasMaxLength(100);
            entity.Property(c => c.FilingFrequencyPlaceholder).HasMaxLength(100);
            entity.Property(c => c.EmployerTaxNotes).HasMaxLength(2000);
            entity.HasMany(c => c.PayrollItems)
                  .WithOne(p => p.Company)
                  .HasForeignKey(p => p.CompanyId);
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
            entity.Property(e => e.DefaultRegularHours).HasPrecision(18, 2);
            entity.Property(e => e.DefaultOvertimeHours).HasPrecision(18, 2);
            entity.Property(e => e.TippedCashWageRate).HasPrecision(18, 2);
            entity.Property(e => e.DependentsAmount).HasPrecision(18, 2);
            entity.Property(e => e.OtherIncome).HasPrecision(18, 2);
            entity.Property(e => e.Deductions).HasPrecision(18, 2);
            entity.Property(e => e.ExtraWithholding).HasPrecision(18, 2);
            entity.Property(e => e.StateAdditionalWithholding).HasPrecision(18, 2);
            entity.Property(e => e.PayrollProfileCreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<EmployeePayrollField>(entity =>
        {
            entity.HasKey(f => f.EmployeePayrollFieldId);
            entity.HasOne(f => f.Employee)
                  .WithMany(e => e.PayrollFields)
                  .HasForeignKey(f => f.EmployeeId);
            entity.Property(f => f.FieldName).HasMaxLength(100);
            entity.Property(f => f.FieldValue).HasMaxLength(1000);
            entity.Property(f => f.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
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
            entity.Property(p => p.PayrollMode).HasMaxLength(50);
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
                  .WithMany(employee => employee.BankAccounts)
                  .HasForeignKey(e => e.EmployeeId);
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.RoutingNumberToken).HasMaxLength(256);
            entity.Property(e => e.AccountNumberToken).HasMaxLength(256);
            entity.Property(e => e.Last4).HasMaxLength(4);
            entity.Property(e => e.DepositAmount).HasPrecision(18, 2);
            entity.Property(e => e.DepositPercent).HasPrecision(5, 2);
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

        modelBuilder.Entity<OfficialSourceDocument>(entity =>
        {
            entity.HasKey(d => d.OfficialSourceDocumentId);
            entity.Property(d => d.SourceName).HasMaxLength(200);
            entity.Property(d => d.PublicationTitle).HasMaxLength(300);
            entity.Property(d => d.OfficialSourceUrl).HasMaxLength(1000);
            entity.Property(d => d.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<OfficialSourceReviewLog>(entity =>
        {
            entity.HasKey(r => r.OfficialSourceReviewLogId);
            entity.HasOne(r => r.OfficialSourceDocument)
                  .WithMany(d => d.ReviewLogs)
                  .HasForeignKey(r => r.OfficialSourceDocumentId);
            entity.Property(r => r.ReviewedByUserId).HasMaxLength(100);
            entity.Property(r => r.Note).HasMaxLength(500);
        });

        modelBuilder.Entity<PayrollItem>(entity =>
        {
            entity.HasKey(p => p.PayrollItemId);
            entity.Property(p => p.ItemCode).HasMaxLength(50);
            entity.Property(p => p.ItemName).HasMaxLength(100);
            entity.Property(p => p.DefaultAmount).HasPrecision(18, 2);
            entity.Property(p => p.DefaultRate).HasPrecision(9, 4);
            entity.Property(p => p.Notes).HasMaxLength(1000);
            entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(p => new { p.CompanyId, p.ItemCode }).IsUnique();
        });

        modelBuilder.Entity<UserDefinedFieldDefinition>(entity =>
        {
            entity.HasKey(f => f.UserDefinedFieldDefinitionId);
            entity.HasOne(f => f.Company)
                  .WithMany()
                  .HasForeignKey(f => f.CompanyId);
            entity.Property(f => f.FieldName).HasMaxLength(100);
            entity.Property(f => f.FieldCode).HasMaxLength(50);
            entity.Property(f => f.DefaultValue).HasMaxLength(1000);
            entity.Property(f => f.ListOptions).HasMaxLength(2000);
            entity.Property(f => f.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(f => new { f.CompanyId, f.FieldCode }).IsUnique();
        });

        modelBuilder.Entity<UserDefinedFieldValue>(entity =>
        {
            entity.HasKey(v => v.UserDefinedFieldValueId);
            entity.HasOne(v => v.FieldDefinition)
                  .WithMany(f => f.Values)
                  .HasForeignKey(v => v.FieldDefinitionId);
            entity.Property(v => v.TextValue).HasMaxLength(4000);
            entity.Property(v => v.DecimalValue).HasPrecision(18, 4);
            entity.Property(v => v.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(v => new { v.FieldDefinitionId, v.EntityType, v.EntityId }).IsUnique();
        });

        modelBuilder.Entity<ImportBatch>(entity =>
        {
            entity.HasKey(b => b.ImportBatchId);
            entity.Property(b => b.FileName).HasMaxLength(260);
            entity.Property(b => b.CreatedByUserId).HasMaxLength(100);
            entity.Property(b => b.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasMany(b => b.Rows)
                  .WithOne(r => r.ImportBatch)
                  .HasForeignKey(r => r.ImportBatchId);
            entity.HasMany(b => b.Errors)
                  .WithOne(e => e.ImportBatch)
                  .HasForeignKey(e => e.ImportBatchId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.HasMany(b => b.Mappings)
                  .WithOne(m => m.ImportBatch)
                  .HasForeignKey(m => m.ImportBatchId);
        });

        modelBuilder.Entity<ImportRow>(entity =>
        {
            entity.HasKey(r => r.ImportRowId);
            entity.Property(r => r.RowDataJson).IsRequired();
            entity.Property(r => r.ErrorSummary).HasMaxLength(500);
            entity.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<ImportError>(entity =>
        {
            entity.HasKey(e => e.ImportErrorId);
            entity.HasOne(e => e.ImportRow)
                  .WithMany()
                  .HasForeignKey(e => e.ImportRowId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.Property(e => e.ColumnName).HasMaxLength(100);
            entity.Property(e => e.ErrorCode).HasMaxLength(100);
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<ImportMapping>(entity =>
        {
            entity.HasKey(m => m.ImportMappingId);
            entity.Property(m => m.SourceColumn).HasMaxLength(100);
            entity.Property(m => m.TargetField).HasMaxLength(100);
            entity.Property(m => m.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<TaxDepositRecord>(entity =>
        {
            entity.HasKey(d => d.TaxDepositRecordId);
            entity.HasOne(d => d.Company)
                  .WithMany()
                  .HasForeignKey(d => d.CompanyId);
            entity.HasOne(d => d.ImportBatch)
                  .WithMany()
                  .HasForeignKey(d => d.ImportBatchId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.Property(d => d.TaxType).HasMaxLength(100);
            entity.Property(d => d.Agency).HasMaxLength(150);
            entity.Property(d => d.Amount).HasPrecision(18, 2);
            entity.Property(d => d.ConfirmationNumber).HasMaxLength(100);
            entity.Property(d => d.PaymentMethod).HasMaxLength(100);
            entity.Property(d => d.Notes).HasMaxLength(1000);
            entity.Property(d => d.RecordSource).HasMaxLength(100);
            entity.Property(d => d.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<W2HistoricalRecord>(entity =>
        {
            entity.HasKey(w => w.W2HistoricalRecordId);
            entity.HasOne(w => w.ImportBatch)
                  .WithMany()
                  .HasForeignKey(w => w.ImportBatchId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.Property(w => w.EmployerName).HasMaxLength(150);
            entity.Property(w => w.EmployerEINMasked).HasMaxLength(20);
            entity.Property(w => w.EmployerAddress).HasMaxLength(300);
            entity.Property(w => w.EmployeeFirstName).HasMaxLength(80);
            entity.Property(w => w.EmployeeLastName).HasMaxLength(80);
            entity.Property(w => w.EmployeeSSNLast4).HasMaxLength(4);
            entity.Property(w => w.EmployeeAddress).HasMaxLength(300);
            entity.Property(w => w.Box1Wages).HasPrecision(18, 2);
            entity.Property(w => w.Box2FederalTaxWithheld).HasPrecision(18, 2);
            entity.Property(w => w.Box3SocialSecurityWages).HasPrecision(18, 2);
            entity.Property(w => w.Box4SocialSecurityTaxWithheld).HasPrecision(18, 2);
            entity.Property(w => w.Box5MedicareWages).HasPrecision(18, 2);
            entity.Property(w => w.Box6MedicareTaxWithheld).HasPrecision(18, 2);
            entity.Property(w => w.Box12CodeAndAmountPlaceholders).HasMaxLength(1000);
            entity.Property(w => w.Box14DescriptionAndAmountPlaceholders).HasMaxLength(1000);
            entity.Property(w => w.StateWages).HasPrecision(18, 2);
            entity.Property(w => w.StateTaxWithheld).HasPrecision(18, 2);
            entity.Property(w => w.LocalWages).HasPrecision(18, 2);
            entity.Property(w => w.LocalTaxWithheld).HasPrecision(18, 2);
            entity.Property(w => w.RecordSource).HasMaxLength(100);
            entity.Property(w => w.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<W2ImportBatch>(entity =>
        {
            entity.HasKey(b => b.W2ImportBatchId);
            entity.HasOne(b => b.Company)
                  .WithMany()
                  .HasForeignKey(b => b.CompanyId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.Property(b => b.FileName).HasMaxLength(260);
            entity.Property(b => b.ImportedByUserId).HasMaxLength(100);
            entity.Property(b => b.Notes).HasMaxLength(1000);
            entity.Property(b => b.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(b => b.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<W2ImportRecord>(entity =>
        {
            entity.HasKey(r => r.W2ImportRecordId);
            entity.HasOne(r => r.W2ImportBatch)
                  .WithMany(b => b.Records)
                  .HasForeignKey(r => r.W2ImportBatchId);
            entity.Property(r => r.EmployerName).HasMaxLength(150);
            entity.Property(r => r.EmployerEINLast4Only).HasMaxLength(4);
            entity.Property(r => r.EmployerAddress).HasMaxLength(300);
            entity.Property(r => r.EmployeeFirstName).HasMaxLength(80);
            entity.Property(r => r.EmployeeMiddleInitial).HasMaxLength(1);
            entity.Property(r => r.EmployeeLastName).HasMaxLength(80);
            entity.Property(r => r.EmployeeSSNLast4Only).HasMaxLength(4);
            entity.Property(r => r.EmployeeAddress).HasMaxLength(300);
            entity.Property(r => r.Box1WagesTipsOtherCompensation).HasPrecision(18, 2);
            entity.Property(r => r.Box2FederalIncomeTaxWithheld).HasPrecision(18, 2);
            entity.Property(r => r.Box3SocialSecurityWages).HasPrecision(18, 2);
            entity.Property(r => r.Box4SocialSecurityTaxWithheld).HasPrecision(18, 2);
            entity.Property(r => r.Box5MedicareWagesAndTips).HasPrecision(18, 2);
            entity.Property(r => r.Box6MedicareTaxWithheld).HasPrecision(18, 2);
            entity.Property(r => r.Box7SocialSecurityTips).HasPrecision(18, 2);
            entity.Property(r => r.Box8AllocatedTips).HasPrecision(18, 2);
            entity.Property(r => r.Box10DependentCareBenefits).HasPrecision(18, 2);
            entity.Property(r => r.Box11NonqualifiedPlans).HasPrecision(18, 2);
            entity.Property(r => r.Box12CodeA).HasMaxLength(4);
            entity.Property(r => r.Box12AmountA).HasPrecision(18, 2);
            entity.Property(r => r.Box12CodeB).HasMaxLength(4);
            entity.Property(r => r.Box12AmountB).HasPrecision(18, 2);
            entity.Property(r => r.Box12CodeC).HasMaxLength(4);
            entity.Property(r => r.Box12AmountC).HasPrecision(18, 2);
            entity.Property(r => r.Box12CodeD).HasMaxLength(4);
            entity.Property(r => r.Box12AmountD).HasPrecision(18, 2);
            entity.Property(r => r.Box14OtherDescription1).HasMaxLength(100);
            entity.Property(r => r.Box14OtherAmount1).HasPrecision(18, 2);
            entity.Property(r => r.Box14OtherDescription2).HasMaxLength(100);
            entity.Property(r => r.Box14OtherAmount2).HasPrecision(18, 2);
            entity.Property(r => r.Box14OtherDescription3).HasMaxLength(100);
            entity.Property(r => r.Box14OtherAmount3).HasPrecision(18, 2);
            entity.Property(r => r.State1).HasMaxLength(2);
            entity.Property(r => r.EmployerStateId1Masked).HasMaxLength(30);
            entity.Property(r => r.StateWages1).HasPrecision(18, 2);
            entity.Property(r => r.StateIncomeTax1).HasPrecision(18, 2);
            entity.Property(r => r.LocalityName1).HasMaxLength(100);
            entity.Property(r => r.LocalWages1).HasPrecision(18, 2);
            entity.Property(r => r.LocalIncomeTax1).HasPrecision(18, 2);
            entity.Property(r => r.State2).HasMaxLength(2);
            entity.Property(r => r.EmployerStateId2Masked).HasMaxLength(30);
            entity.Property(r => r.StateWages2).HasPrecision(18, 2);
            entity.Property(r => r.StateIncomeTax2).HasPrecision(18, 2);
            entity.Property(r => r.LocalityName2).HasMaxLength(100);
            entity.Property(r => r.LocalWages2).HasPrecision(18, 2);
            entity.Property(r => r.LocalIncomeTax2).HasPrecision(18, 2);
            entity.Property(r => r.RawExtractedTextPreview).HasMaxLength(2000);
            entity.Property(r => r.ConfidenceScore).HasPrecision(5, 4);
            entity.Property(r => r.ReviewNotes).HasMaxLength(1000);
            entity.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(r => r.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<W2ImportError>(entity =>
        {
            entity.HasKey(e => e.W2ImportErrorId);
            entity.HasOne(e => e.W2ImportBatch)
                  .WithMany(b => b.Errors)
                  .HasForeignKey(e => e.W2ImportBatchId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.W2ImportRecord)
                  .WithMany()
                  .HasForeignKey(e => e.W2ImportRecordId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.Property(e => e.FieldName).HasMaxLength(100);
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
