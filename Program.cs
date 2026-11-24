using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using LedgerAI.Api.AI.Agents;
using LedgerAI.Api.AI.Monitoring;
using LedgerAI.Api.AI.Pipelines;
using LedgerAI.Api.AI.Routers;
using LedgerAI.Api.Communication.WhatsApp;
using LedgerAI.Api.Communication.Workers;
using LedgerAI.Api.Core.Interfaces;
using LedgerAI.Api.Core.Security;
using LedgerAI.Api.Data.Context;
using LedgerAI.Api.Data.Interceptors;
using LedgerAI.Api.Data.Repositories;
using LedgerAI.Api.Data.UnitOfWork;
using LedgerAI.Api.Hubs;
using LedgerAI.Api.Modules.Accounts;
using LedgerAI.Api.Modules.Catalog;
using LedgerAI.Api.Modules.Inventory;
using LedgerAI.Api.Modules.Invoicing;
using LedgerAI.Api.Modules.Orders;
using LedgerAI.Api.Modules.TenantAdmin;

var builder = WebApplication.CreateBuilder(args);

// 1. Controller & Presentation Services
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

// 2. Multi-Tenant Context & Security
builder.Services.AddScoped<TenantContext>();
builder.Services.AddScoped<ITenantProvider>(sp => sp.GetRequiredService<TenantContext>());
builder.Services.AddScoped<TenantRlsInterceptor>();

// 3. Persistence & Unit of Work
builder.Services.AddDbContext<LedgerDbContext>((sp, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var interceptor = sp.GetRequiredService<TenantRlsInterceptor>();
    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention()
           .AddInterceptors(interceptor);
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// 4. Domain Feature Modules (Service Stubs)
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

// 5. AI Multi-Agent & Orchestration Stubs
builder.Services.AddScoped<IntentExtractorAgent>();
builder.Services.AddScoped<SupervisorAgent>();
builder.Services.AddScoped<OrderAgent>();
builder.Services.AddScoped<LedgerAgent>();
builder.Services.AddScoped<IAiTelemetryService, AiDecisionLogger>();
builder.Services.AddScoped<LlmFailoverRouter>();
builder.Services.AddSingleton<TextSanitizerPipeline>();
builder.Services.AddSingleton<IntentValidationPipeline>();

// 6. External Communication & Background Workers
builder.Services.AddHttpClient<WhatsAppClient>();
builder.Services.AddSingleton<WebhookSignatureVerifier>();
builder.Services.AddHostedService<OutboxDispatcherWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapHub<LedgerHub>("/hubs/ledger");
app.MapControllers();

app.Run();
