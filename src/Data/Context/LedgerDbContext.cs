using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Core.Enums;
using LedgerAI.Api.Modules.Accounts;
using LedgerAI.Api.Modules.Catalog;
using LedgerAI.Api.Modules.Inventory;
using LedgerAI.Api.Modules.Invoicing;
using LedgerAI.Api.Modules.Operations;
using LedgerAI.Api.Modules.Orders;
using LedgerAI.Api.Modules.TenantAdmin;

namespace LedgerAI.Api.Data.Context;

public class LedgerDbContext : DbContext
{
    private static readonly StockMovementTypeConverter StockMovementTypeConv = new();
    private static readonly OrderStatusConverter OrderStatusConv = new();
    private static readonly ChargeDirectionConverter ChargeDirectionConv = new();
    private static readonly AccountingTreatmentConverter AccountingTreatmentConv = new();

    public LedgerDbContext(DbContextOptions<LedgerDbContext> options) : base(options)
    {
    }

    // Tenant & Identity
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();

    // Catalog
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
    public DbSet<Product> Products => Set<Product>();

    // Inventory
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<StockItem> StockItems => Set<StockItem>();
    public DbSet<StockBatch> StockBatches => Set<StockBatch>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<StockReservation> StockReservations => Set<StockReservation>();

    // Accounts & Ledger
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    public DbSet<LedgerAllocation> LedgerAllocations => Set<LedgerAllocation>();

    // Orders
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    // Invoicing
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<InvoiceCharge> InvoiceCharges => Set<InvoiceCharge>();

    // Operations & Audit
    public DbSet<InboundMessage> InboundMessages => Set<InboundMessage>();
    public DbSet<OutboxEvent> OutboxEvents => Set<OutboxEvent>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LedgerDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(entityType =>
                         typeof(BaseEntity).IsAssignableFrom(entityType.ClrType) &&
                         entityType.ClrType != typeof(User)))
        {
            modelBuilder.Entity(entityType.ClrType)
                .HasOne(typeof(Tenant), "Tenant")
                .WithMany()
                .HasForeignKey("TenantId")
                .OnDelete(DeleteBehavior.Restrict);
        }

        modelBuilder.Entity<User>()
            .HasOne(user => user.Tenant)
            .WithMany(tenant => tenant.Users)
            .HasForeignKey(user => user.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasIndex(entity => new { entity.TenantId, entity.IsActive })
            .HasDatabaseName("idx_users_tenant_active");
        modelBuilder.Entity<User>()
            .HasIndex(entity => new { entity.TenantId, entity.UserCode })
            .IsUnique();

        modelBuilder.Entity<UnitOfMeasure>(entity =>
        {
            entity.Property(unit => unit.AliasesJson).HasColumnName("aliases").HasColumnType("jsonb");
            entity.HasIndex(unit => unit.AliasesJson)
                .HasMethod("gin")
                .HasDatabaseName("idx_uom_aliases_gin");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(product => product.AttributesJson).HasColumnName("attributes").HasColumnType("jsonb");
            entity.Property(product => product.CategoryId).IsRequired(false);
            entity.Property(product => product.PackSize).HasPrecision(18, 4);
            entity.Property(product => product.DefaultPurchasePrice).HasPrecision(18, 4);
            entity.Property(product => product.WeightedAvgCost).HasPrecision(18, 4);
            entity.Property(product => product.DefaultSalesPrice).HasPrecision(18, 4);
            entity.Property(product => product.MinimumStock).HasPrecision(18, 4);
            entity.HasIndex(product => product.AttributesJson)
                .HasMethod("gin")
                .HasDatabaseName("idx_products_attributes_gin");
            entity.HasIndex(product => new { product.TenantId, product.CategoryId })
                .HasDatabaseName("idx_products_category");
            entity.HasOne(product => product.Category).WithMany(category => category.Products)
                .HasForeignKey(product => product.CategoryId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(product => product.BaseUnit).WithMany()
                .HasForeignKey(product => product.BaseUnitId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(product => product.SecondaryUnit).WithMany()
                .HasForeignKey(product => product.SecondaryUnitId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.Property(account => account.AccountType)
                .HasConversion(
                    value => value.ToString().ToUpperInvariant(),
                    value => Enum.Parse<AccountType>(value, true))
                .HasMaxLength(30);
            entity.Property(account => account.RiskTier)
                .HasConversion(
                    value => value.ToString().ToUpperInvariant(),
                    value => Enum.Parse<RiskTier>(value, true))
                .HasMaxLength(20);
            entity.Property(account => account.MetadataJson).HasColumnName("metadata").HasColumnType("jsonb");
            entity.Property(account => account.OpeningBalance).HasPrecision(18, 4);
            entity.Property(account => account.CurrentBalance).HasPrecision(18, 4);
            entity.Property(account => account.CreditLimit).HasPrecision(18, 4);
            entity.Property(account => account.ReservedCredit).HasPrecision(18, 4);
            entity.HasIndex(account => new { account.TenantId, account.AccountType, account.IsActive })
                .HasDatabaseName("idx_accounts_tenant_type_active");
            entity.HasIndex(account => new { account.TenantId, account.PhoneNo })
                .HasDatabaseName("idx_accounts_tenant_phone");
            entity.HasIndex(account => new { account.TenantId, account.MobileNo })
                .HasDatabaseName("idx_accounts_tenant_mobile");
        });

        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.Property(stockItem => stockItem.QuantityOnHand).HasPrecision(18, 4);
            entity.Property(stockItem => stockItem.QuantityReserved).HasPrecision(18, 4);
            entity.HasIndex(stockItem => new { stockItem.TenantId, stockItem.WarehouseId, stockItem.ProductId }).IsUnique();
            entity.HasIndex(stockItem => new { stockItem.TenantId, stockItem.ProductId })
                .HasDatabaseName("idx_stock_items_product");
            entity.HasOne(stockItem => stockItem.Warehouse).WithMany()
                .HasForeignKey(stockItem => stockItem.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(stockItem => stockItem.Product).WithMany()
                .HasForeignKey(stockItem => stockItem.ProductId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StockBatch>(entity =>
        {
            entity.Property(batch => batch.UnitCost).HasPrecision(18, 4);
            entity.Property(batch => batch.QuantityOnHand).HasPrecision(18, 4);
            entity.Property(batch => batch.QuantityReserved).HasPrecision(18, 4);
            entity.HasIndex(batch => new { batch.TenantId, batch.Id, batch.WarehouseId, batch.ProductId }).IsUnique();
            entity.HasIndex(batch => new { batch.TenantId, batch.WarehouseId, batch.ProductId, batch.ExpiresAt, batch.CreatedAt })
                .HasDatabaseName("idx_stock_batches_fefo");
            entity.HasOne(batch => batch.Warehouse).WithMany()
                .HasForeignKey(batch => batch.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(batch => batch.Product).WithMany()
                .HasForeignKey(batch => batch.ProductId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.Property(movement => movement.MovementType)
                .HasConversion(StockMovementTypeConv)
                .HasMaxLength(30);
            entity.Property(movement => movement.QuantityDelta).HasPrecision(18, 4);
            entity.Property(movement => movement.UnitCost).HasPrecision(18, 4);
            entity.HasIndex(movement => new { movement.TenantId, movement.ProductId, movement.CreatedAt })
                .HasDatabaseName("idx_stock_movements_product");
            entity.HasIndex(movement => new { movement.TenantId, movement.ReferenceType, movement.ReferenceId })
                .HasDatabaseName("idx_stock_movements_ref");
            entity.HasOne(movement => movement.Warehouse).WithMany()
                .HasForeignKey(movement => movement.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(movement => movement.Product).WithMany()
                .HasForeignKey(movement => movement.ProductId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(movement => movement.Batch).WithMany()
                .HasForeignKey(movement => movement.BatchId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(order => order.OrderStatus)
                .HasConversion(OrderStatusConv)
                .HasMaxLength(30);
            entity.Property(order => order.PaymentTerms)
                .HasConversion(
                    value => value.ToString().ToUpperInvariant(),
                    value => Enum.Parse<PaymentTerms>(value, true))
                .HasMaxLength(20);
            entity.Property(order => order.TotalAmount).HasPrecision(18, 4);
            entity.Property(order => order.UpfrontPaidAmount).HasPrecision(18, 4);
            entity.Property(order => order.AiConfidenceScore).HasPrecision(5, 4);
            entity.Property(order => order.ExecutionMetadataJson).HasColumnName("execution_metadata").HasColumnType("jsonb");
            entity.HasIndex(order => new { order.TenantId, order.OrderStatus, order.CreatedAt })
                .HasDatabaseName("idx_orders_tenant_status_created");
            entity.HasIndex(order => new { order.TenantId, order.AccountId, order.CreatedAt })
                .HasDatabaseName("idx_orders_account_created");
            entity.HasOne(order => order.Account).WithMany()
                .HasForeignKey(order => order.AccountId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(item => item.OrderedQuantity).HasPrecision(18, 4);
            entity.Property(item => item.OrderedUnitPrice).HasPrecision(18, 4);
            entity.Property(item => item.ConversionFactor).HasPrecision(18, 6);
            entity.Property(item => item.BaseQuantity).HasPrecision(18, 4);
            entity.Property(item => item.UnitCostAtSale).HasPrecision(18, 4);
            entity.Property(item => item.TotalPrice).HasPrecision(18, 4)
                .HasComputedColumnSql("round(ordered_quantity * ordered_unit_price, 4)", stored: true);
            entity.Property(item => item.TotalCost).HasPrecision(18, 4)
                .HasComputedColumnSql("round(base_quantity * unit_cost_at_sale, 4)", stored: true);
            entity.HasIndex(item => new { item.TenantId, item.OrderId })
                .HasDatabaseName("idx_order_items_order");
            entity.HasIndex(item => new { item.TenantId, item.ProductId })
                .HasDatabaseName("idx_order_items_product");
            entity.HasOne(item => item.Order).WithMany(order => order.Items)
                .HasForeignKey(item => item.OrderId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(item => item.Product).WithMany()
                .HasForeignKey(item => item.ProductId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(item => item.Warehouse).WithMany()
                .HasForeignKey(item => item.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(item => item.Batch).WithMany()
                .HasForeignKey(item => item.BatchId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(item => item.OrderedUnit).WithMany()
                .HasForeignKey(item => item.OrderedUnitId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StockReservation>(entity =>
        {
            entity.Property(reservation => reservation.ReservedQuantity).HasPrecision(18, 4);
            entity.Property(reservation => reservation.ReleasedQuantity).HasPrecision(18, 4);
            entity.Property(reservation => reservation.CommittedQuantity).HasPrecision(18, 4);
            entity.HasIndex(reservation => new { reservation.TenantId, reservation.OrderId, reservation.Status })
                .HasDatabaseName("idx_stock_reservations_order");
            entity.HasIndex(reservation => new { reservation.TenantId, reservation.BatchId, reservation.Status })
                .HasDatabaseName("idx_stock_reservations_batch");
            entity.HasOne<Order>().WithMany()
                .HasForeignKey(reservation => reservation.OrderId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<OrderItem>().WithMany()
                .HasForeignKey(reservation => reservation.OrderItemId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(reservation => reservation.Warehouse).WithMany()
                .HasForeignKey(reservation => reservation.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(reservation => reservation.Product).WithMany()
                .HasForeignKey(reservation => reservation.ProductId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(reservation => reservation.Batch).WithMany()
                .HasForeignKey(reservation => reservation.BatchId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.Property(invoice => invoice.InvoiceType)
                .HasConversion(
                    value => value.ToString().ToUpperInvariant(),
                    value => Enum.Parse<InvoiceType>(value, true))
                .HasMaxLength(20);
            entity.Property(invoice => invoice.VoucherSeries)
                .HasColumnType("character(2)")
                .HasComputedColumnSql("CASE WHEN invoice_type = 'PURCHASE' THEN '00' ELSE '01' END", stored: true);
            entity.Property(invoice => invoice.BalanceDue).HasPrecision(18, 4)
                .HasComputedColumnSql("GREATEST(grand_total - amount_paid, 0)", stored: true);
            entity.Property(invoice => invoice.SubtotalAmount).HasPrecision(18, 4);
            entity.Property(invoice => invoice.DiscountAmount).HasPrecision(18, 4);
            entity.Property(invoice => invoice.TaxAmount).HasPrecision(18, 4);
            entity.Property(invoice => invoice.GrandTotal).HasPrecision(18, 4);
            entity.Property(invoice => invoice.AmountPaid).HasPrecision(18, 4);
            entity.HasIndex(invoice => new { invoice.TenantId, invoice.InvoiceType, invoice.AccountId, invoice.InvoiceStatus, invoice.IssuedAt })
                .HasDatabaseName("idx_invoices_type_account_status");
            entity.HasIndex(invoice => new { invoice.TenantId, invoice.OrderId })
                .HasDatabaseName("idx_invoices_order");
            entity.HasOne(invoice => invoice.Order).WithMany()
                .HasForeignKey(invoice => invoice.OrderId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(invoice => invoice.Account).WithMany()
                .HasForeignKey(invoice => invoice.AccountId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(invoice => invoice.TransporterAccount).WithMany()
                .HasForeignKey(invoice => invoice.TransporterAccountId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.Property(item => item.Quantity).HasPrecision(18, 4);
            entity.Property(item => item.UnitPrice).HasPrecision(18, 4);
            entity.Property(item => item.DiscountAmount).HasPrecision(18, 4);
            entity.Property(item => item.TaxRate).HasPrecision(6, 4);
            entity.Property(item => item.TaxAmount).HasPrecision(18, 4);
            entity.Property(item => item.ConversionFactor).HasPrecision(18, 6);
            entity.Property(item => item.BaseQuantity).HasPrecision(18, 4);
            entity.Property(item => item.BaseUnitCost).HasPrecision(18, 4);
            entity.Property(item => item.LineTotal).HasPrecision(18, 4)
                .HasComputedColumnSql("round((quantity * unit_price) - discount_amount + tax_amount, 4)", stored: true);
            entity.Property(item => item.LineCogs).HasPrecision(18, 4)
                .HasComputedColumnSql("round(base_quantity * base_unit_cost, 4)", stored: true);
            entity.HasIndex(item => new { item.TenantId, item.InvoiceId })
                .HasDatabaseName("idx_invoice_items_invoice");
            entity.HasOne(item => item.Invoice).WithMany(invoice => invoice.Items)
                .HasForeignKey(item => item.InvoiceId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(item => item.OrderItem).WithMany()
                .HasForeignKey(item => item.OrderItemId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(item => item.Warehouse).WithMany()
                .HasForeignKey(item => item.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(item => item.Product).WithMany()
                .HasForeignKey(item => item.ProductId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(item => item.Batch).WithMany()
                .HasForeignKey(item => item.BatchId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(item => item.Unit).WithMany()
                .HasForeignKey(item => item.UnitId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InvoiceCharge>(entity =>
        {
            entity.Property(charge => charge.ChargeDirection)
                .HasConversion(ChargeDirectionConv)
                .HasMaxLength(20);
            entity.Property(charge => charge.AccountingTreatment)
                .HasConversion(AccountingTreatmentConv)
                .HasMaxLength(30);
            entity.Property(charge => charge.Amount).HasPrecision(18, 4);
            entity.HasIndex(charge => new { charge.TenantId, charge.InvoiceId })
                .HasDatabaseName("idx_invoice_charges_invoice");
            entity.HasOne(charge => charge.Invoice).WithMany(invoice => invoice.Charges)
                .HasForeignKey(charge => charge.InvoiceId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(charge => charge.PayableAccount).WithMany()
                .HasForeignKey(charge => charge.PayableAccountId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LedgerEntry>(entity =>
        {
            entity.Property(entry => entry.Amount).HasPrecision(18, 4);
            entity.Property(entry => entry.RunningBalance).HasPrecision(18, 4);
            entity.HasIndex(entry => new { entry.TenantId, entry.AccountId, entry.CreatedAt })
                .HasDatabaseName("idx_ledger_account_created");
            entity.HasIndex(entry => new { entry.TenantId, entry.OrderId })
                .HasDatabaseName("idx_ledger_order");
            entity.HasIndex(entry => new { entry.TenantId, entry.InvoiceId })
                .HasDatabaseName("idx_ledger_invoice");
            entity.HasOne(entry => entry.Account).WithMany()
                .HasForeignKey(entry => entry.AccountId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(entry => entry.ReversesEntry).WithMany()
                .HasForeignKey(entry => entry.ReversesEntryId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LedgerAllocation>(entity =>
        {
            entity.Property(allocation => allocation.Amount).HasPrecision(18, 4);
            entity.HasIndex(allocation => new { allocation.TenantId, allocation.CreditEntryId })
                .HasDatabaseName("idx_allocations_credit");
            entity.HasIndex(allocation => new { allocation.TenantId, allocation.DebitEntryId })
                .HasDatabaseName("idx_allocations_debit");
            entity.HasOne(allocation => allocation.Account).WithMany()
                .HasForeignKey(allocation => allocation.AccountId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(allocation => allocation.DebitEntry).WithMany()
                .HasForeignKey(allocation => allocation.DebitEntryId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(allocation => allocation.CreditEntry).WithMany()
                .HasForeignKey(allocation => allocation.CreditEntryId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InboundMessage>(entity =>
        {
            entity.Property(message => message.ProcessingStatus)
                .HasConversion(
                    value => value.ToString().ToUpperInvariant(),
                    value => Enum.Parse<InboundProcessingStatus>(value, true))
                .HasMaxLength(30);
            entity.Property(message => message.RawPayloadJson).HasColumnName("raw_payload").HasColumnType("jsonb");
            entity.Property(message => message.AiConfidenceScore).HasPrecision(5, 4);
            entity.HasIndex(message => new { message.TenantId, message.ReceivedAt })
                .HasDatabaseName("idx_inbound_tenant_received");
        });

        modelBuilder.Entity<OutboxEvent>(entity =>
        {
            entity.Property(message => message.PayloadJson).HasColumnName("payload").HasColumnType("jsonb");
            entity.HasIndex(message => new { message.AvailableAt, message.CreatedAt })
                .HasDatabaseName("idx_outbox_ready");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.Property(log => log.OldValuesJson).HasColumnName("old_values").HasColumnType("jsonb");
            entity.Property(log => log.NewValuesJson).HasColumnName("new_values").HasColumnType("jsonb");
            entity.HasIndex(log => new { log.TenantId, log.EntityName, log.EntityId, log.CreatedAt })
                .HasDatabaseName("idx_audit_entity");
        });
    }

    private sealed class StockMovementTypeConverter() : ValueConverter<StockMovementType, string>(
        value => ToDb(value),
        value => FromDb(value))
    {
        private static string ToDb(StockMovementType value) => value switch
        {
            StockMovementType.SalesReturn => "SALES_RETURN",
            StockMovementType.PurchaseReturn => "PURCHASE_RETURN",
            StockMovementType.TransferIn => "TRANSFER_IN",
            StockMovementType.TransferOut => "TRANSFER_OUT",
            StockMovementType.DamageExpiry => "DAMAGE_EXPIRY",
            StockMovementType.Purchase => "PURCHASE",
            StockMovementType.Sale => "SALE",
            StockMovementType.Adjustment => "ADJUSTMENT",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown stock movement type.")
        };

        private static StockMovementType FromDb(string value) => value switch
        {
            "SALES_RETURN" => StockMovementType.SalesReturn,
            "PURCHASE_RETURN" => StockMovementType.PurchaseReturn,
            "TRANSFER_IN" => StockMovementType.TransferIn,
            "TRANSFER_OUT" => StockMovementType.TransferOut,
            "DAMAGE_EXPIRY" => StockMovementType.DamageExpiry,
            "PURCHASE" => StockMovementType.Purchase,
            "SALE" => StockMovementType.Sale,
            "ADJUSTMENT" => StockMovementType.Adjustment,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown stock movement type.")
        };
    }

    private sealed class OrderStatusConverter() : ValueConverter<OrderStatus, string>(
        value => ToDb(value),
        value => FromDb(value))
    {
        private static string ToDb(OrderStatus value) =>
            value == OrderStatus.PendingApproval
                ? "PENDING_APPROVAL"
                : value.ToString().ToUpperInvariant();

        private static OrderStatus FromDb(string value) =>
            value == "PENDING_APPROVAL"
                ? OrderStatus.PendingApproval
                : Enum.Parse<OrderStatus>(value, true);
    }

    private sealed class ChargeDirectionConverter() : ValueConverter<ChargeDirection, string>(
        value => ToDb(value),
        value => FromDb(value))
    {
        private static string ToDb(ChargeDirection value) => value switch
        {
            ChargeDirection.AddToBill => "ADD_TO_BILL",
            ChargeDirection.DeductFromBill => "DEDUCT_FROM_BILL",
            ChargeDirection.MemoOnly => "MEMO_ONLY",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown charge direction.")
        };

        private static ChargeDirection FromDb(string value) => value switch
        {
            "ADD_TO_BILL" => ChargeDirection.AddToBill,
            "DEDUCT_FROM_BILL" => ChargeDirection.DeductFromBill,
            "MEMO_ONLY" => ChargeDirection.MemoOnly,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown charge direction.")
        };
    }

    private sealed class AccountingTreatmentConverter() : ValueConverter<AccountingTreatment, string>(
        value => ToDb(value),
        value => FromDb(value))
    {
        private static string ToDb(AccountingTreatment value) => value switch
        {
            AccountingTreatment.CapitalizeLandedCost => "CAPITALIZE_LANDED_COST",
            AccountingTreatment.FulfillmentExpense => "FULFILLMENT_EXPENSE",
            AccountingTreatment.TaxPassThrough => "TAX_PASS_THROUGH",
            AccountingTreatment.DiscountWaiver => "DISCOUNT_WAIVER",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown accounting treatment.")
        };

        private static AccountingTreatment FromDb(string value) => value switch
        {
            "CAPITALIZE_LANDED_COST" => AccountingTreatment.CapitalizeLandedCost,
            "FULFILLMENT_EXPENSE" => AccountingTreatment.FulfillmentExpense,
            "TAX_PASS_THROUGH" => AccountingTreatment.TaxPassThrough,
            "DISCOUNT_WAIVER" => AccountingTreatment.DiscountWaiver,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown accounting treatment.")
        };
    }
}