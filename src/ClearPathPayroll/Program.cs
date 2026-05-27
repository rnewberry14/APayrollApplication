using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using ClearPathPayroll.Components;
using ClearPathPayroll.Configuration;
using ClearPathPayroll.Data;
using ClearPathPayroll.Integrations;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true);
}

var keyVaultUri = builder.Configuration["AzureKeyVault:VaultUri"] ?? Environment.GetEnvironmentVariable("AZURE_KEY_VAULT_URI");
if (!builder.Environment.IsDevelopment() && !string.IsNullOrWhiteSpace(keyVaultUri))
{
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
}

static bool HasProductionSecret(string? value) => SecretValidationHelper.IsProductionValueConfigured(value);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOptions<ConnectionStringsOptions>()
    .Bind(builder.Configuration.GetSection("ConnectionStrings"))
    .ValidateDataAnnotations()
    .Validate(options => !builder.Environment.IsProduction() || HasProductionSecret(options.DefaultConnection), "A real DefaultConnection must be configured in production.")
    .ValidateOnStart();

builder.Services.AddOptions<TaxApiOptions>()
    .Bind(builder.Configuration.GetSection("TaxApi"))
    .ValidateDataAnnotations()
    .Validate(options => !builder.Environment.IsProduction() || HasProductionSecret(options.BaseUrl), "A real Tax API base URL must be configured in production.")
    .Validate(options => !builder.Environment.IsProduction() || HasProductionSecret(options.ApiKey), "A real Tax API key must be configured in production.")
    .ValidateOnStart();

builder.Services.AddOptions<AchApiOptions>()
    .Bind(builder.Configuration.GetSection("ACHApi"))
    .ValidateDataAnnotations()
    .Validate(options => !builder.Environment.IsProduction() || HasProductionSecret(options.BaseUrl), "A real ACH API base URL must be configured in production.")
    .Validate(options => !builder.Environment.IsProduction() || HasProductionSecret(options.ApiKey), "A real ACH API key must be configured in production.")
    .Validate(options => !builder.Environment.IsProduction() || options.AllowProductionSubmission, "Production ACH submission requires AllowProductionSubmission to be set.")
    .Validate(options => !builder.Environment.IsDevelopment() || options.SandboxMode, "Development environment must use ACH SandboxMode.")
    .ValidateOnStart();

builder.Services.AddOptions<BankVerificationOptions>()
    .Bind(builder.Configuration.GetSection("BankVerification"))
    .ValidateDataAnnotations()
    .Validate(options => !builder.Environment.IsProduction() || HasProductionSecret(options.ApiKey), "A real bank verification API key must be configured in production.")
    .Validate(options => !builder.Environment.IsProduction() || HasProductionSecret(options.BaseUrl), "A real bank verification base URL must be configured in production.")
    .ValidateOnStart();

builder.Services.AddOptions<EncryptionOptions>()
    .Bind(builder.Configuration.GetSection("Encryption"))
    .Validate(options => !builder.Environment.IsProduction() || options.HasConfiguredKey(), "Encryption key material or Key Vault URI must be configured in production.")
    .ValidateOnStart();

builder.Services.AddOptions<AuthenticationOptions>()
    .Bind(builder.Configuration.GetSection("Authentication"))
    .ValidateDataAnnotations()
    .Validate(options => !builder.Environment.IsProduction() || HasProductionSecret(options.Authority), "Authentication authority must be configured in production.")
    .ValidateOnStart();

builder.Services.AddOptions<EmailOptions>()
    .Bind(builder.Configuration.GetSection("Email"))
    .Validate(options => !builder.Environment.IsProduction() || HasProductionSecret(options.ApiKey), "Email provider API key must be configured in production.")
    .Validate(options => !builder.Environment.IsProduction() || HasProductionSecret(options.FromAddress), "Email FromAddress must be configured in production.")
    .ValidateOnStart();

builder.Services.AddOptions<AzureKeyVaultOptions>()
    .Bind(builder.Configuration.GetSection("AzureKeyVault"))
    .Validate(options => !builder.Environment.IsProduction() || options.Enabled, "Azure Key Vault must be enabled in production.")
    .Validate(options => !builder.Environment.IsProduction() || HasProductionSecret(options.VaultUri), "Azure Key Vault URI must be configured in production.")
    .ValidateOnStart();

builder.Services.AddOptions<AzureStorageOptions>()
    .Bind(builder.Configuration.GetSection("AzureStorage"));

builder.Services.AddOptions<PrototypeModeOptions>()
    .Bind(builder.Configuration.GetSection("PrototypeMode"))
    .ValidateDataAnnotations()
    .Validate(options => !builder.Environment.IsProduction() || !options.Enabled, "Prototype Mode may not be enabled in production.")
    .ValidateOnStart();

builder.Services.AddOptions<LimitedLiabilityModeOptions>()
    .Bind(builder.Configuration.GetSection("LimitedLiabilityMode"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var prototypeOptions = builder.Configuration.GetSection("PrototypeMode").Get<PrototypeModeOptions>() ?? new PrototypeModeOptions();
var localPrototypeEnabled = PrototypeModeHelper.ShouldUseLocalPrototypeMode(builder.Environment, prototypeOptions);
var localPrototypeConnectionString = string.Empty;

// Add DbContext
if (localPrototypeEnabled)
{
    localPrototypeConnectionString = PrototypeModeHelper.GetLocalDbConnectionString(prototypeOptions.LocalDbDatabaseName);
    builder.Services.AddDbContext<PayrollDbContext>(options =>
        options.UseSqlServer(localPrototypeConnectionString));
}
else
{
    builder.Services.AddDbContext<PayrollDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Register integrations
builder.Services.AddScoped<ITaxApiService, TaxApiService>();
builder.Services.AddScoped<IACHApiService, ACHApiService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddHttpClient<ITaxCalculationService, PayrollTaxApiCalculationService>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<TaxApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/'));
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
    if (options.SandboxMode)
    {
        client.DefaultRequestHeaders.Add("X-Sandbox-Mode", "true");
    }
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddScoped<IAchPaymentService, FakeAchPaymentService>();

// Register services
builder.Services.AddScoped<CompanyService>();
builder.Services.AddScoped<PayrollItemService>();
builder.Services.AddScoped<UserDefinedFieldService>();
builder.Services.AddSingleton<PayrollFieldTemplateCatalog>();
builder.Services.AddSingleton<QuickBooksImportTemplateCatalog>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<PayScheduleService>();
builder.Services.AddScoped<PayrollService>();
builder.Services.AddScoped<PayrollRegisterService>();
builder.Services.AddScoped<TaxLiabilityReportService>();
builder.Services.AddScoped<GrossPayCalculationService>();
builder.Services.AddScoped<DeductionCalculationService>();
builder.Services.AddScoped<PayrollCalculationService>();
builder.Services.AddScoped<CheckCalculationPreviewService>();
builder.Services.AddScoped<PayrollApprovalService>();
builder.Services.AddScoped<DirectDepositSubmissionService>();
builder.Services.AddScoped<PayStubService>();
builder.Services.AddScoped<PaycheckPrintService>();
builder.Services.AddScoped<SeedDataService>();
builder.Services.AddScoped<DemoDataSeeder>();
builder.Services.AddScoped<OfficialSourceService>();
builder.Services.AddScoped<IImportFileParser, CsvImportFileParser>();
builder.Services.AddScoped<IImportFileParser, TabDelimitedImportFileParser>();
builder.Services.AddScoped<IImportFileParser, ExcelImportFileParser>();
builder.Services.AddScoped<ImportService>();
builder.Services.AddScoped<EmployeesOnlyImportService>();
builder.Services.AddScoped<ChecksImportService>();
builder.Services.AddScoped<TaxDepositsImportService>();
builder.Services.AddScoped<W2ImportService>();
builder.Services.AddScoped<W2PdfTextParser>();
builder.Services.AddScoped<W2PdfImportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

if (localPrototypeEnabled)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Local Prototype Mode enabled. Using local database '{DatabaseName}' on the user's machine.", prototypeOptions.LocalDbDatabaseName);
}

app.Run();
