namespace LedgerAI.Api.Core.Enums;

public enum AccountingTreatment
{
    CapitalizeLandedCost, // Added into product WAC on Purchase
    FulfillmentExpense,   // Deducted from Gross Profit on Sale
    TaxPassThrough,       // FBR WHT 236G/236H (does not distort product margin)
    DiscountWaiver        // Kasar / settlement concession
}
