using System;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PO;
using System.Collections.Generic;
using PX.Common;
using System.Linq;

namespace PrecisionCust
{
    public class MarkupMaint : PXGraph<MarkupMaint>
    {
        public PXSave<MarkupParentTbl> Save;
        public PXCancel<MarkupParentTbl> Cancel;

        #region Constant Values
        //used for combobox MarkupType
        private const string Amount = "A";
        private const string Percent = "P";

        //used for combobox ReplenishmentCost Values
        public const string LastPrice = "L";
        public const string AverageCost = "A";
        public const string VendorPrice = "V";
        #endregion

        #region Selects + Dataview Delegate

        public SelectFrom<MarkupParentTbl>.View MarkupParent; // for ParentUse Only Autonumbering of LineNbr

        public SelectFrom<MarkupPricing>.OrderBy<Asc<MarkupPricing.lineNbr>>.View MarkupDet;

        #endregion

        #region EventHandlers

        protected void _(Events.RowSelected<MarkupPricing> e)
        {
            var row = e.Row as MarkupPricing;
            if (row == null) return;
            PXCache sender = e.Cache;
            bool isAvgPrice = false;
            if (row.EffectiveDate != null && row.ExpirationDate != null)
            {
                if (Accessinfo.BusinessDate > row.ExpirationDate)
                {
                    // Acuminator disable once PX1050 HardcodedStringInLocalizationMethod [Justification]
                    sender.RaiseExceptionHandling<MarkupPricing.effectiveDate>(row, row.EffectiveDate, new PXSetPropertyException("Markup Pricing is expired.", PXErrorLevel.RowWarning));
                }
                if (row.EffectiveDate > row.ExpirationDate) //Warning notification for Dates
                {
                    // Acuminator disable once PX1050 HardcodedStringInLocalizationMethod [Justification]
                    sender.RaiseExceptionHandling<MarkupPricing.effectiveDate>(row, row.EffectiveDate, new PXSetPropertyException("Effective Date must not be greater than Expiration Date.", PXErrorLevel.Warning));
                }
            }
            if (row.InventoryID != null)
            {
                InventoryItem item = SelectFrom<InventoryItem>.
                  Where<InventoryItem.inventoryID.IsEqual<@P.AsInt>>.View.Select(this, row.InventoryID);
                if (item != null)
                {
                    InventoryItemCurySettings invCurSet = SelectFrom<InventoryItemCurySettings>.
                      Where<InventoryItemCurySettings.inventoryID.IsEqual<@P.AsInt>>.View.Select(this, item.InventoryID);
                    if (invCurSet != null)
                    {
                        var invCurSetExt = PXCache<InventoryItemCurySettings>.GetExtension<InventoryItemCurySettingsExtPC>(invCurSet);
                        if (invCurSetExt.UsrReplacementCst == "A") isAvgPrice = true;
                        if (String.IsNullOrEmpty(invCurSetExt.UsrReplacementCst))
                        {
                            // Acuminator disable once PX1050 HardcodedStringInLocalizationMethod [Justification]
                            sender.RaiseExceptionHandling<MarkupPricing.replacementCost>(row, row.ReplacementCost, new PXSetPropertyException("There is no Replacement Cost value on Item {0}", PXErrorLevel.Warning, item.InventoryCD)); // warning notification if there is no setup value in stock items
                        }
                    }
                }
            }

            //PXUIFieldAttribute.SetRequired<MarkupPricing.siteID>(sender, isAvgPrice == true);
            PXUIFieldAttribute.SetEnabled<MarkupPricing.siteID>(sender, row, isAvgPrice == true);
        }

        protected void _(Events.FieldDefaulting<MarkupPricing, MarkupPricing.salesOrderPrice> e)
        {
            var row = e.Row as MarkupPricing;
            if (row == null) return;
            PXCache sender = e.Cache;
            decimal? price = 0m;

            if (row.MarkupType != null && row.MarkupValue != 0m)
            {
                if (row.MarkupType == Percent && row.InventoryID != null)
                {
                    price = ((row.MarkupValue / 100) * row.ReplacementCost) + row.ReplacementCost;
                }
                else if (row.MarkupType == Amount && row.InventoryID != null)
                {
                    price = row.MarkupValue + row.ReplacementCost;
                }
            }
            e.NewValue = price ?? 0m;
        }

        protected void _(Events.FieldDefaulting<MarkupPricing, MarkupPricing.replacementCost> e) // get default value from Inventory
        {
            var row = e.Row as MarkupPricing;
            if (row == null) return;
            PXCache sender = e.Cache;
            decimal? repCost = 0m;
            if (row.InventoryID != null)
            {
                InventoryItemCurySettings invCurSet = SelectFrom<InventoryItemCurySettings>.
                  Where<InventoryItemCurySettings.inventoryID.IsEqual<@P.AsInt>>.View.Select(this, row.InventoryID);
                var invCurSetExt = PXCache<InventoryItemCurySettings>.GetExtension<InventoryItemCurySettingsExtPC>(invCurSet);
                if (!String.IsNullOrEmpty(invCurSetExt.UsrReplacementCst))
                {
                    if (invCurSetExt.UsrReplacementCst == LastPrice)
                    {
                        INItemCost itemCst = SelectFrom<INItemCost>.
                          Where<INItemCost.inventoryID.IsEqual<@P.AsInt>.
                          And<INItemCost.curyID.IsEqual<@P.AsString>>>.View.Select(this, invCurSet.InventoryID, invCurSet.CuryID); // get last price from Stock Items Last Cost
                        if (itemCst != null)
                        {
                            repCost = itemCst.LastCost;
                        }
                    }
                    else if (invCurSetExt.UsrReplacementCst == AverageCost && row.SiteID != null)
                    {
                        INItemSite itemSite = SelectFrom<INItemSite>.
                          Where<INItemSite.inventoryID.IsEqual<@P.AsInt>.
                          And<INItemSite.siteID.IsEqual<@P.AsInt>>>.View.Select(this, row.InventoryID, row.SiteID); // get default avg cost from default warehouse of item.
                        if (itemSite != null)
                        {
                            repCost = itemSite.AvgCost;
                        }
                    }
                    else if (invCurSetExt.UsrReplacementCst == VendorPrice)
                    {
                        if (invCurSet.PreferredVendorID != null) // default vendor of item.
                        {
                            POVendorInventory poVendor = SelectFrom<POVendorInventory>.
                              Where<POVendorInventory.vendorID.IsEqual<@P.AsInt>.
                              And<POVendorInventory.inventoryID.IsEqual<@P.AsInt>>>.View.Select(this, invCurSet.PreferredVendorID, invCurSet.InventoryID);
                            if (poVendor != null)
                            {
                                repCost = poVendor.LastPrice;
                            }
                        }
                    }
                }
            }
            e.NewValue = repCost;
        }

        protected void _(Events.FieldUpdated<MarkupPricing, MarkupPricing.inventoryID> e)
        {
            var row = e.Row as MarkupPricing;
            if (row == null) return;
            PXCache sender = e.Cache;
            sender.SetDefaultExt<MarkupPricing.replacementCost>(row); // trigger field defaulting for Replacement Cost field
            sender.SetDefaultExt<MarkupPricing.salesOrderPrice>(row); // trigger field defaulting in SalesOrderPrice
        }

        protected void _(Events.FieldUpdated<MarkupPricing, MarkupPricing.markupType> e)
        {
            var row = e.Row as MarkupPricing;
            if (row == null) return;
            PXCache sender = e.Cache;
            sender.SetDefaultExt<MarkupPricing.replacementCost>(row);
            sender.SetDefaultExt<MarkupPricing.salesOrderPrice>(row); // trigger field defaulting in SalesOrderPrice
        }
        protected void _(Events.FieldUpdated<MarkupPricing, MarkupPricing.siteID> e)
        {
            var row = e.Row as MarkupPricing;
            if (row == null) return;
            PXCache sender = e.Cache;
            sender.SetDefaultExt<MarkupPricing.replacementCost>(row); // trigger field defaulting for Replacement Cost field
            sender.SetDefaultExt<MarkupPricing.salesOrderPrice>(row); // trigger field defaulting in SalesOrderPrice
        }
        protected void _(Events.FieldUpdated<MarkupPricing, MarkupPricing.markupValue> e)
        {
            var row = e.Row as MarkupPricing;
            if (row == null) return;
            PXCache sender = e.Cache;
            sender.SetDefaultExt<MarkupPricing.replacementCost>(row);
            sender.SetDefaultExt<MarkupPricing.salesOrderPrice>(row); // trigger field defaulting in SalesOrderPrice
        }
        protected void _(Events.FieldUpdated<MarkupPricing, MarkupPricing.itemClassID> e)
        {
            var row = e.Row as MarkupPricing;
            if (row == null) return;
            PXCache sender = e.Cache;
            if (row.ItemClassID != null)
            {
                sender.SetValue<MarkupPricing.siteID>(row, null);
            }
        }

        protected void _(Events.RowPersisting<MarkupPricing> e)
        {
            var row = e.Row as MarkupPricing;
            if (row == null) return;

            if (e.Operation != PXDBOperation.Delete)
            {
                PXCache sender = e.Cache;
                bool hasError = false;
                ValidateMarkupPricingUniqueness();
                if (row.InventoryID != null)
                {
                    InventoryItemCurySettings invCurSet = SelectFrom<InventoryItemCurySettings>.
                      Where<InventoryItemCurySettings.inventoryID.IsEqual<@P.AsInt>>.View.Select(this, row.InventoryID);
                    if (invCurSet != null)
                    {
                        var invCurSetExt = PXCache<InventoryItemCurySettings>.GetExtension<InventoryItemCurySettingsExtPC>(invCurSet);
                        if (invCurSetExt.UsrReplacementCst == "A")
                        {
                            if (row.SiteID == null)
                            {
                                hasError = true;
                                // Acuminator disable once PX1050 HardcodedStringInLocalizationMethod [Justification]
                                sender.RaiseExceptionHandling<MarkupPricing.siteID>(row, row.SiteID, new PXSetPropertyException("Error: Warehouse cannot be empty.", PXErrorLevel.Error));
                            }
                        }
                    }
                }
                if (hasError == true)
                {
                    // Acuminator disable once PX1050 HardcodedStringInLocalizationMethod [Justification]
                    //throw new PXException("Error: Inserting 'MarkupPricing' record raised at least one error. Please review the errors.");
                }
            }
        }

        private void ValidateMarkupPricingUniqueness() {
            var records = MarkupDet.Select().RowCast<MarkupPricing>().ToList();

            var duplicates = records.GroupBy(r => new
            {
                r.CustomerID,
                r.BranchID,
                r.ItemClassID,
                r.SiteID,
                r.InventoryID,
                r.QtyBreak,
                r.EffectiveDate,
                r.MarkupType
            })
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

            if (duplicates.Any()) {
                throw new PXException("Duplicate records are not allowed.");
            }
        }

        #endregion

    }
}