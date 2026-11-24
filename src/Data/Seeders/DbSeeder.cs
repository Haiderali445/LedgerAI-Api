using System.Security.Cryptography;
using System.Text;
using LedgerAI.Api.Core.Enums;
using LedgerAI.Api.Data.Context;
using LedgerAI.Api.Modules.Accounts;
using LedgerAI.Api.Modules.Catalog;
using LedgerAI.Api.Modules.Inventory;
using LedgerAI.Api.Modules.Invoicing;
using LedgerAI.Api.Modules.Operations;
using LedgerAI.Api.Modules.Orders;
using LedgerAI.Api.Modules.TenantAdmin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LedgerAI.Api.Data.Seeders;

public static class DbSeeder
{
    private static readonly Guid MasterTenantId = Guid.Parse("11111111-1111-4111-8111-111111111111");
    private static readonly DateTimeOffset SeedTimestamp = new(2026, 11, 1, 9, 0, 0, TimeSpan.Zero);

    public static async Task SeedAsync(
        LedgerDbContext context,
        ILogger logger,
        bool includeMockData,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        await context.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT set_config('app.current_tenant', {MasterTenantId.ToString()}, true)",
            cancellationToken);

        var tenant = await context.Tenants.SingleOrDefaultAsync(
            item => item.Id == MasterTenantId || item.TenantCode == "MASTER-PKR",
            cancellationToken);

        if (tenant is null)
        {
            tenant = new Tenant
            {
                Id = MasterTenantId,
                TenantCode = "MASTER-PKR",
                BusinessName = "LedgerAI Master Wholesale",
                DomainSlug = "master-pkr",
                CurrencyCode = "PKR",
                IsActive = true,
                CreatedAt = SeedTimestamp
            };
            context.Tenants.Add(tenant);
        }

        var owner = await EnsureUserAsync(
            context,
            tenant.Id,
            "user:owner",
            "OWNER-001",
            "Master Owner",
            "owner@ledgerai.local",
            "Owner",
            cancellationToken);

        await EnsureUserAsync(
            context,
            tenant.Id,
            "user:ADMIN-001",
            "ADMIN-001",
            "Master Admin",
            "admin@ledgerai.local",
            "Admin",
            cancellationToken);

        await EnsureUserAsync(
            context,
            tenant.Id,
            "user:OPERATOR-001",
            "OPERATOR-001",
            "Warehouse Operator",
            "operator@ledgerai.local",
            "Operator",
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        if (!includeMockData)
        {
            await transaction.CommitAsync(cancellationToken);
            logger.LogInformation("Seeded master tenant {TenantCode} and master users", tenant.TenantCode);
            return;
        }

        var baseBag = await EnsureCategoryAsync(context, tenant.Id, "BAGS", "Wholesale Bags", cancellationToken);
        var uomBori = await EnsureUomAsync(context, tenant.Id, "BORI", "Bori", true, """["bori","ٹھھیلی"]""", cancellationToken);
        var uomKg = await EnsureUomAsync(context, tenant.Id, "KG", "Kilogram", true, """["kg","کلو"]""", cancellationToken);

        var warehouse = await EnsureEntityAsync(
            context,
            () => context.Warehouses.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id && item.WarehouseCode == "WH-MAIN",
                cancellationToken),
            () => new Warehouse
            {
                Id = DeterministicGuid("warehouse:main"),
                TenantId = tenant.Id,
                WarehouseCode = "WH-MAIN",
                Name = "Main Faisalabad Warehouse",
                Address = "Faisalabad Wholesale Market",
                IsActive = true,
                CreatedAt = SeedTimestamp
            },
            cancellationToken);

        var product = await EnsureEntityAsync(
            context,
            () => context.Products.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id && item.ProductCode == "RICE-SUPER",
                cancellationToken),
            () => new Product
            {
                Id = DeterministicGuid("product:rice-super"),
                TenantId = tenant.Id,
                ProductCode = "RICE-SUPER",
                Name = "Super Basmati Rice",
                CategoryId = baseBag.Id,
                BaseUnitId = uomKg.Id,
                SecondaryUnitId = uomBori.Id,
                PackSize = 50m,
                Barcode = "PK-RICE-001",
                DefaultPurchasePrice = 15500m,
                WeightedAvgCost = 15500m,
                DefaultSalesPrice = 17000m,
                MinimumStock = 100m,
                AttributesJson = """{"aliases":["super basmati","rice","چاول"]}""",
                IsActive = true,
                IsDeleted = false,
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp
            },
            cancellationToken);

        // 1000 kg purchased (20 bori @ 50 kg), 500 kg committed to initial sale -> 500 kg remaining on hand
        var batch = await EnsureEntityAsync(
            context,
            () => context.StockBatches.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id && item.BatchCode == "RICE-2026-A",
                cancellationToken),
            () => new StockBatch
            {
                Id = DeterministicGuid("batch:rice-2026-a"),
                TenantId = tenant.Id,
                WarehouseId = warehouse.Id,
                ProductId = product.Id,
                BatchCode = "RICE-2026-A",
                UnitCost = 15500m,
                QuantityOnHand = 500m,
                QuantityReserved = 0m,
                ManufacturedAt = new DateOnly(2026, 10, 15),
                ExpiresAt = new DateOnly(2027, 12, 31),
                CreatedAt = SeedTimestamp
            },
            cancellationToken);

        await EnsureEntityAsync(
            context,
            () => context.StockMovements.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id &&
                         item.ReferenceType == "SEED_PURCHASE" &&
                         item.ReferenceId == batch.Id,
                cancellationToken),
            () => new StockMovement
            {
                Id = DeterministicGuid("movement:rice-opening-purchase"),
                TenantId = tenant.Id,
                WarehouseId = warehouse.Id,
                ProductId = product.Id,
                BatchId = batch.Id,
                MovementType = StockMovementType.Purchase,
                QuantityDelta = 1000m,
                UnitCost = 15500m,
                ReferenceType = "SEED_PURCHASE",
                ReferenceId = batch.Id,
                Notes = "Idempotent opening stock purchase (1000 kg)",
                CreatedBy = owner.Id,
                CreatedAt = SeedTimestamp
            },
            cancellationToken);

        var customer = await EnsureEntityAsync(
            context,
            () => context.Accounts.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id && item.AccountCode == "CUST-001",
                cancellationToken),
            () => new Account
            {
                Id = DeterministicGuid("account:customer"),
                TenantId = tenant.Id,
                AccountCode = "CUST-001",
                AccountType = AccountType.Customer,
                Name = "Al-Rehman Traders",
                MobileNo = "03001234567",
                Address = "Karkhana Bazaar, Faisalabad",
                OpeningBalance = 10000m,
                OpeningBalanceType = "DR",
                CurrentBalance = 90000m, // 10,000 opening DR + 90,000 invoice DR - 10,000 upfront CR
                CreditLimit = 250000m,
                ReservedCredit = 0m,
                RiskTier = RiskTier.Medium,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = owner.Id,
                CreatedAt = SeedTimestamp
            },
            cancellationToken);

        await EnsureEntityAsync(
            context,
            () => context.Accounts.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id && item.AccountCode == "SUP-001",
                cancellationToken),
            () => new Account
            {
                Id = DeterministicGuid("account:supplier"),
                TenantId = tenant.Id,
                AccountCode = "SUP-001",
                AccountType = AccountType.Supplier,
                Name = "Punjab Rice Mills",
                MobileNo = "03007654321",
                OpeningBalance = 0m,
                OpeningBalanceType = "CR",
                CurrentBalance = 0m,
                CreditLimit = 500000m,
                ReservedCredit = 0m,
                RiskTier = RiskTier.Low,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = owner.Id,
                CreatedAt = SeedTimestamp
            },
            cancellationToken);

        var transporter = await EnsureEntityAsync(
            context,
            () => context.Accounts.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id && item.AccountCode == "TRANS-001",
                cancellationToken),
            () => new Account
            {
                Id = DeterministicGuid("account:transporter"),
                TenantId = tenant.Id,
                AccountCode = "TRANS-001",
                AccountType = AccountType.Transporter,
                Name = "FastLine Transport",
                MobileNo = "03111111111",
                OpeningBalance = 0m,
                CurrentBalance = 0m,
                CreditLimit = 100000m,
                ReservedCredit = 0m,
                RiskTier = RiskTier.Low,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = owner.Id,
                CreatedAt = SeedTimestamp
            },
            cancellationToken);

        var order = await EnsureEntityAsync(
            context,
            () => context.Orders.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id && item.IdempotencyKey == "seed-order-001",
                cancellationToken),
            () => new Order
            {
                Id = DeterministicGuid("order:seed-001"),
                TenantId = tenant.Id,
                OrderCode = "ORD-2026-001",
                AccountId = customer.Id,
                OrderStatus = OrderStatus.Committed,
                PaymentTerms = PaymentTerms.Credit,
                TotalAmount = 85000m,
                UpfrontPaidAmount = 10000m,
                AiConfidenceScore = 0.95m,
                IdempotencyKey = "seed-order-001",
                RawIntentSummary = "10 bori super basmati rice",
                CreatedAt = SeedTimestamp,
                CommittedAt = SeedTimestamp,
                CreatedBy = owner.Id
            },
            cancellationToken);

        var orderItem = await EnsureEntityAsync(
            context,
            () => context.OrderItems.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id && item.OrderId == order.Id && item.ProductId == product.Id,
                cancellationToken),
            () => new OrderItem
            {
                Id = DeterministicGuid("order-item:seed-001"),
                TenantId = tenant.Id,
                OrderId = order.Id,
                ProductId = product.Id,
                WarehouseId = warehouse.Id,
                BatchId = batch.Id,
                OrderedUnitId = uomBori.Id,
                OrderedQuantity = 10m,
                OrderedUnitPrice = 8500m,
                ConversionFactor = 50m,
                BaseQuantity = 500m,
                UnitCostAtSale = 15500m,
                CreatedAt = SeedTimestamp
            },
            cancellationToken);

        await EnsureEntityAsync(
            context,
            () => context.StockReservations.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id &&
                         item.OrderItemId == orderItem.Id &&
                         item.BatchId == batch.Id,
                cancellationToken),
            () => new StockReservation
            {
                Id = DeterministicGuid("reservation:seed-001"),
                TenantId = tenant.Id,
                OrderId = order.Id,
                OrderItemId = orderItem.Id,
                WarehouseId = warehouse.Id,
                ProductId = product.Id,
                BatchId = batch.Id,
                ReservedQuantity = 500m,
                ReleasedQuantity = 0m,
                CommittedQuantity = 500m,
                Status = "COMMITTED",
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp
            },
            cancellationToken);

        var invoice = await EnsureEntityAsync(
            context,
            () => context.Invoices.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id && item.IdempotencyKey == "seed-invoice-001",
                cancellationToken),
            () => new Invoice
            {
                Id = DeterministicGuid("invoice:sale-001"),
                TenantId = tenant.Id,
                InvoiceType = InvoiceType.Sale,
                InvoiceNumber = "01-2026-001",
                OrderId = order.Id,
                AccountId = customer.Id,
                WarehouseId = warehouse.Id,
                TransporterAccountId = transporter.Id,
                BiltyNumber = "BLT-2026-001",
                InvoiceStatus = "ISSUED",
                SubtotalAmount = 85000m,
                DiscountAmount = 0m,
                TaxAmount = 0m,
                GrandTotal = 90000m,
                AmountPaid = 10000m,
                DueDate = new DateOnly(2026, 12, 1),
                IdempotencyKey = "seed-invoice-001",
                IssuedBy = owner.Id,
                IssuedAt = SeedTimestamp,
                CreatedAt = SeedTimestamp
            },
            cancellationToken);

        await EnsureEntityAsync(
            context,
            () => context.StockMovements.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id &&
                         item.ReferenceType == "SEED_SALE_INVOICE" &&
                         item.ReferenceId == invoice.Id,
                cancellationToken),
            () => new StockMovement
            {
                Id = DeterministicGuid("movement:rice-sale-001"),
                TenantId = tenant.Id,
                WarehouseId = warehouse.Id,
                ProductId = product.Id,
                BatchId = batch.Id,
                MovementType = StockMovementType.Sale,
                QuantityDelta = -500m,
                UnitCost = 15500m,
                ReferenceType = "SEED_SALE_INVOICE",
                ReferenceId = invoice.Id,
                Notes = "Committed sale stock movement for 01-2026-001 (-500 kg)",
                CreatedBy = owner.Id,
                CreatedAt = SeedTimestamp
            },
            cancellationToken);

        await EnsureEntityAsync(
            context,
            () => context.InvoiceItems.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id &&
                         item.InvoiceId == invoice.Id &&
                         item.OrderItemId == orderItem.Id,
                cancellationToken),
            () => new InvoiceItem
            {
                Id = DeterministicGuid("invoice-item:sale-001"),
                TenantId = tenant.Id,
                InvoiceId = invoice.Id,
                OrderItemId = orderItem.Id,
                WarehouseId = warehouse.Id,
                ProductId = product.Id,
                BatchId = batch.Id,
                UnitId = uomBori.Id,
                Quantity = 10m,
                UnitPrice = 8500m,
                ConversionFactor = 50m,
                BaseQuantity = 500m,
                BaseUnitCost = 15500m,
                CreatedAt = SeedTimestamp
            },
            cancellationToken);

        await EnsureEntityAsync(
            context,
            () => context.InvoiceCharges.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id &&
                         item.InvoiceId == invoice.Id &&
                         item.ChargeType == "FREIGHT_BILTY",
                cancellationToken),
            () => new InvoiceCharge
            {
                Id = DeterministicGuid("invoice-charge:freight-001"),
                TenantId = tenant.Id,
                InvoiceId = invoice.Id,
                ChargeType = "FREIGHT_BILTY",
                ChargeDirection = ChargeDirection.AddToBill,
                AccountingTreatment = AccountingTreatment.FulfillmentExpense,
                PayableAccountId = transporter.Id,
                Amount = 5000m,
                Notes = "Seed freight charge on sale invoice",
                CreatedAt = SeedTimestamp
            },
            cancellationToken);

        var debitEntry = await EnsureEntityAsync(
            context,
            () => context.LedgerEntries.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id && item.IdempotencyKey == "seed-ledger-debit-001",
                cancellationToken),
            () => new LedgerEntry
            {
                Id = DeterministicGuid("ledger:debit-001"),
                TenantId = tenant.Id,
                AccountId = customer.Id,
                OrderId = order.Id,
                InvoiceId = invoice.Id,
                EntryType = "DEBIT",
                Amount = 90000m,
                RunningBalance = 100000m,
                IdempotencyKey = "seed-ledger-debit-001",
                Notes = "Seed invoice debit",
                CreatedBy = owner.Id,
                CreatedAt = SeedTimestamp
            },
            cancellationToken);

        var creditEntry = await EnsureEntityAsync(
            context,
            () => context.LedgerEntries.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id && item.IdempotencyKey == "seed-ledger-credit-001",
                cancellationToken),
            () => new LedgerEntry
            {
                Id = DeterministicGuid("ledger:credit-001"),
                TenantId = tenant.Id,
                AccountId = customer.Id,
                OrderId = order.Id,
                InvoiceId = invoice.Id,
                EntryType = "CREDIT",
                Amount = 10000m,
                RunningBalance = 90000m,
                IdempotencyKey = "seed-ledger-credit-001",
                Notes = "Seed upfront payment",
                CreatedBy = owner.Id,
                CreatedAt = SeedTimestamp.AddMinutes(1)
            },
            cancellationToken);

        await EnsureEntityAsync(
            context,
            () => context.LedgerAllocations.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id &&
                         item.DebitEntryId == debitEntry.Id &&
                         item.CreditEntryId == creditEntry.Id,
                cancellationToken),
            () => new LedgerAllocation
            {
                Id = DeterministicGuid("allocation:seed-001"),
                TenantId = tenant.Id,
                AccountId = customer.Id,
                DebitEntryId = debitEntry.Id,
                CreditEntryId = creditEntry.Id,
                Amount = 10000m,
                CreatedBy = owner.Id,
                CreatedAt = SeedTimestamp.AddMinutes(1)
            },
            cancellationToken);

        await EnsureEntityAsync(
            context,
            () => context.InboundMessages.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id && item.Wamid == "wamid-seed-001",
                cancellationToken),
            () => new InboundMessage
            {
                Id = DeterministicGuid("inbound:seed-001"),
                TenantId = tenant.Id,
                Wamid = "wamid-seed-001",
                SenderPhone = "923001234567",
                Channel = "WhatsApp",
                RawPayloadJson = """{"type":"text","body":"10 bori super basmati rice"}""",
                TranscriptionText = "10 bori super basmati rice",
                ProcessingStatus = InboundProcessingStatus.Processed,
                MatchedOrderId = order.Id,
                AiConfidenceScore = 0.95m,
                ReceivedAt = SeedTimestamp.AddMinutes(-5),
                ProcessedAt = SeedTimestamp.AddMinutes(-4),
                CreatedAt = SeedTimestamp.AddMinutes(-5)
            },
            cancellationToken);

        await EnsureEntityAsync(
            context,
            () => context.OutboxEvents.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id && item.DedupeKey == "seed-invoice-created-001",
                cancellationToken),
            () => new OutboxEvent
            {
                Id = DeterministicGuid("outbox:seed-001"),
                TenantId = tenant.Id,
                EventType = "INVOICE_CREATED",
                DedupeKey = "seed-invoice-created-001",
                PayloadJson = $$"""{"invoiceId":"{{invoice.Id}}","tenantId":"{{tenant.Id}}"}""",
                AvailableAt = SeedTimestamp,
                ProcessedAt = SeedTimestamp,
                CreatedAt = SeedTimestamp
            },
            cancellationToken);

        await EnsureEntityAsync(
            context,
            () => context.AuditLogs.SingleOrDefaultAsync(
                item => item.TenantId == tenant.Id &&
                         item.EntityName == "Invoice" &&
                         item.EntityId == invoice.Id,
                cancellationToken),
            () => new AuditLog
            {
                Id = DeterministicGuid("audit:seed-invoice-001"),
                TenantId = tenant.Id,
                UserId = owner.Id,
                ActorSnapshot = "MASTER_OWNER",
                ActionTaken = "SEED_INVOICE_CREATED",
                EntityName = "Invoice",
                EntityId = invoice.Id,
                NewValuesJson = """{"seed":true}""",
                CreatedAt = SeedTimestamp
            },
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation(
            "Seeded master tenant {TenantCode} with coherent mock data",
            tenant.TenantCode);
    }

    private static async Task<User> EnsureUserAsync(
        LedgerDbContext context,
        Guid tenantId,
        string deterministicKey,
        string userCode,
        string fullName,
        string email,
        string role,
        CancellationToken cancellationToken)
    {
        return await EnsureEntityAsync(
            context,
            () => context.Users.SingleOrDefaultAsync(
                item => item.TenantId == tenantId && item.UserCode == userCode,
                cancellationToken),
            () => new User
            {
                Id = DeterministicUserGuid(deterministicKey),
                TenantId = tenantId,
                UserCode = userCode,
                FullName = fullName,
                Email = email,
                PasswordHash = HashPassword("LedgerAI@2026!"),
                Role = role,
                IsActive = true,
                CreatedAt = SeedTimestamp
            },
            cancellationToken);
    }

    private static async Task<Category> EnsureCategoryAsync(
        LedgerDbContext context,
        Guid tenantId,
        string name,
        string description,
        CancellationToken cancellationToken)
    {
        return await EnsureEntityAsync(
            context,
            () => context.Categories.SingleOrDefaultAsync(
                item => item.TenantId == tenantId && item.Name == name,
                cancellationToken),
            () => new Category
            {
                Id = DeterministicGuid($"category:{name}"),
                TenantId = tenantId,
                Name = name,
                Description = description,
                IsActive = true,
                CreatedAt = SeedTimestamp
            },
            cancellationToken);
    }

    private static async Task<UnitOfMeasure> EnsureUomAsync(
        LedgerDbContext context,
        Guid tenantId,
        string code,
        string name,
        bool fractional,
        string aliases,
        CancellationToken cancellationToken)
    {
        return await EnsureEntityAsync(
            context,
            () => context.UnitsOfMeasure.SingleOrDefaultAsync(
                item => item.TenantId == tenantId && item.UnitCode == code,
                cancellationToken),
            () => new UnitOfMeasure
            {
                Id = DeterministicGuid($"uom:{code}"),
                TenantId = tenantId,
                UnitCode = code,
                Name = name,
                AllowFractional = fractional,
                AliasesJson = aliases,
                IsActive = true,
                CreatedAt = SeedTimestamp
            },
            cancellationToken);
    }

    private static async Task<T> EnsureEntityAsync<T>(
        LedgerDbContext context,
        Func<Task<T?>> find,
        Func<T> create,
        CancellationToken cancellationToken)
        where T : class
    {
        var existing = await find();
        if (existing is not null)
            return existing;

        var entity = create();
        await context.AddAsync(entity, cancellationToken);
        return entity;
    }

    private static string HashPassword(string plainText)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            plainText,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    private static Guid DeterministicGuid(string value)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        var bytes = hash.Take(16).ToArray();
        return new Guid(bytes);
    }

    private static Guid DeterministicUserGuid(string value)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(value));
        return Guid.ParseExact(Convert.ToHexString(hash), "N");
    }
}